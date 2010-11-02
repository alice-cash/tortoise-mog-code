/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/29/2010
 * Time: 10:28 PM
 * 
 * Copyright 2010 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using C5;

using Tortoise.Shared.Module;
using Tortoise.Shared.Collection;
using Tortoise.Shared.IO;
using Tortoise.Shared.Net;

namespace Tortoise.Shared.Net
{
    /// <summary>
    /// Base connection for connectivity between 2 hosts.
    /// </summary>
    class Connection
    {
        public const int MTU = 1337;

        protected static Dictionary<ushort, IComModule> _moduleActions = new Dictionary<ushort, IComModule>();
        public static void AddModuleHandle(ushort ID, IComModule module)
        {
            if (_moduleActions.ContainsKey(ID))
                throw new Exception("ID already exists!");
            _moduleActions.Add(ID, module);
        }

        private Queue<Packet> _packetQue = new Queue<Packet>();


        public System.EventHandler<MessageEventArgs> MessageEvent;

        public static ConnectionState ConnectionState = ConnectionState.NotConnected;

        private bool _readyForData;

        public bool ReadyForData
        {
            get { return _readyForData; }
        }

        protected NetworkStream _ns;
        protected Socket _sock;

        //  protected BinaryReader _br;
        //  protected BinaryWriter _bw;

        //private byte[] _data;
        protected ushort _length;
        protected DateTime _recived;

        protected byte[] _passKey = null;

        protected string _authKey;

        public string AuthKey
        {
            get { return _authKey; }
        }

        public EventHandler DisconnectedEvent;

        private bool _DisconnectedEventCalled;

        // private bool _endRecived;
        // private bool _writeRecived;

        private bool _isasync;


        private DateTime _dataRecivedTime = DateTime.Now;
        private ulong _dataRecivedCount = 0;
        private ulong _lastCount;
        private LimitedList<ulong> _last15Seconds;

        byte[] _bf = new byte[MTU];

        public ulong Last15PacketSpeed
        {
            get { return CalculateAverage(_last15Seconds); }
        }

        public ulong LastPacketSpeed
        {
            get { return _lastCount; }
        }

        public bool Connected
        {
            get { return _sock == null ? false : _sock.Connected; }
        }

        public int PacketQueSize
        {
            get { return _packetQue.Count; }
        }

        public Connection(Socket connection)
        {
            _sock = connection;
            Init();
        }

        public Connection(IPAddress dest, int port)
        {
            _sock = new Socket(dest.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sock.Connect(dest, port);
            Init();
        }


        public Connection(IPAddress dest, int port, byte[] passkey)
            : this(dest, port)
        {
            _passKey = passkey;
        }

        public Connection(string dest, int port)
            : this(IPAddress.Parse(dest), port)
        {

        }

        public Connection(Socket connection, byte[] passkey)
            : this(connection)
        {
            _passKey = passkey;

        }

        private void Init()
        {
            _ns = new NetworkStream(_sock);
            _DisconnectedEventCalled = false;
            _authKey = "";
            _last15Seconds = new LimitedList<ulong>(15, 0);
            _dataRecivedTime = DateTime.Now;
            _isasync = false;

        }


        public void Poll()
        {
            if (!Connected)
            {
                Disconnected();
                return;
            }
            Poll_Read();
            //Check incase a read kicked us.
            if (!Connected)
            {
                Disconnected();
                return;
            }
            Poll_Write();

            if (DateTime.Now - _dataRecivedTime >= TimeSpan.FromSeconds(1))
            {
                _last15Seconds.Add(_dataRecivedCount);
                _lastCount = _dataRecivedCount;
                _dataRecivedCount = 0;
                _dataRecivedTime = DateTime.Now;
            }
        }

        private void Poll_Read()
        {

            int count = 0;
            
            int available = _sock.Available;
            do
            {
                if (_length == 0)
                {
                    if (available >= 2)
                    {
                        _ns.Read(_bf, 0, 2);
                        _length = BitConverter.ToUInt16(_bf, 0);

                    }
                }
                available = _sock.Available;
                if (_length > 0 && available >= _length)
                {
                    _ns.Read(_bf, 0, _length);
                    ByteReader br = new ByteReader(_bf, 0, _length);

                    //this should succeed, always....
                    var pTempID = br.ReadUShort();
                    if (!pTempID.Sucess)
                    {
                        SyncError();
                        return;
                    }
                    PacketID pID;
                    if (!PacketIDHelper.TryParse(pTempID.Result, out pID))
                    {
                        SyncError();
                        return;
                    }

                    //remove 2 from the length that we just read from
                    HandleInput(br, Convert.ToUInt16(_length - 2), pID);

                    _dataRecivedCount += 1;
                    _length = 0;
                }
                count++;
                if (count >= 20)
                    break;
            } while (_length != 0);
            /*
            var asyncResult = _ns.BeginRead(_bf, 0, 2, (IAsyncResult result) =>
                 {

                     var subsAyncResult = _ns.BeginRead(_bf, 0, _length, (IAsyncResult subresult) =>
                       {
                          
                           _isasync = false;

                       }, null);

                     
                 }, null);*/
        }

        private void Poll_Write()
        {
            lock (_packetQue)
            {
                int count = 0;
                ByteWriter bw = new ByteWriter();
                int length = 0;
                while (_packetQue.Count > 0)
                {

                    Packet p;

                    p = _packetQue.Peek();

                    if (p.Data.Length > MTU)
                        //We throw an Exception here because this is clearly a logic error on our end.
                        //Some @#$%&^* didn't implement something right, somewhere, probably me.
                        throw new Exception("PACKETS MUST BE SMALLER THAN THE MTU!!!!!!!!");
                    if (p.Data.Length + length < MTU)
                    {
                        //Dequeue it here
                    }
                    else
                        break;
                    length += p.Data.Length;
                    bw.Write(p.Data);
                    count++;
                }
                byte[] data = bw.GetArray();
                _ns.Write(data, 0, data.Length);

            }

        }

        private void AddPacket(byte[] data)
        {
            lock (_packetQue)
            {
                _packetQue.Enqueue(new Packet(data));
            }
        }

        protected void HandleInput(ByteReader br, ushort length, PacketID pID)
        {

            //Switch through all of the items, even if we need to throw a SyncError.
            //Otherwise each ID should call a Read_{DescritiveInfo}()
            //The reason for the empty SyncError() for a release is we don't care about
            //reasons. We can assume the end developer has
            Dictionary<String, Object> debugData;
            switch (pID)
            {
                case PacketID.Null:
                    debugData = new Dictionary<String, Object>();
                    debugData.Add("PacketID", PacketID.Null);
                    SyncError(debugData);

                    break;
                case PacketID.Message:
                    Read_Message(br);
                    break;
                case PacketID.Key:
                    break;
                case PacketID.ModulePacket:
                    Read_ModulePacket(br);
                    break;
            }
        }


        void Read_Key(ByteReader br)
        {
            var tmp = br.ReadString();
            if (!tmp.Sucess)
            {
                SyncError();
                return;
            }
            //finish?
        }

        void Read_Message(ByteReader br)
        {
            //(MessageID reason)
            //Check for a valid Enum Item.

            var rTmp = br.ReadUShort();
            if (!rTmp.Sucess)
            {
                SyncError();
                return;
            }
            MessageID mID = MessageID.Null;
            if (!MessageIDHelper.TryParse(rTmp.Result, out mID))
            {
                Disconnect(MessageID.SyncError);
                return;
            }
            if (MessageEvent != null)
                MessageEvent(this, new MessageEventArgs(mID));
        }

        void Read_ModulePacket(ByteReader br)
        {
            var moduleID = br.ReadUShort();
            if (!moduleID.Sucess)
            {
                SyncError();
                return;
            }
            if (!_moduleActions.ContainsKey(moduleID.Result))
            {
                SyncError();
                throw new Exception("moduleID not registered!");
            }

            //remove 2 from the length because we just read 2 off
            _moduleActions[moduleID.Result].Communication(this, br);
        }

        public void Write_ModulePacket(byte[] data, ushort moduleID)
        {
            //2 for ID, 2 for module ID, x for data length
            ushort length = Convert.ToUInt16(4 + data.Length);
            ByteWriter bw = new ByteWriter();

            bw.Write(length);
            bw.Write(PacketID.ModulePacket.Value());
            bw.Write(moduleID);
            bw.Write(data);
            AddPacket(bw.GetArray());
        }

        public void Write_Message(MessageID reason)
        {
            //2 for ID, 2 for message ID
            ushort length = 4;
            ByteWriter bw = new ByteWriter();

            bw.Write(length);
            bw.Write(PacketID.Message.Value());
            bw.Write(reason.Value());

            AddPacket(bw.GetArray());
        }

        /// <summary>
        /// Disconnects the client without any reason.
        /// </summary>
        public void Disconnect()
        {
            _sock.Close();
            Disconnected();
        }

        private ulong CalculateAverage(LimitedList<ulong> items)
        {
            double count = 0, total = 0, result = 0;
            foreach (var v in items)
            {
                if (v == 0) continue;
                count++;
                total += v;
            }


            result = total / count;
            if (double.IsNaN(result))
                result = 0;
            return Convert.ToUInt64(result);
        }


        private void Disconnected()
        {
            if (Connected || _DisconnectedEventCalled) return; //UUh, we are not disconnected or this has been called already.
            if (DisconnectedEvent != null)
                DisconnectedEvent(this, EventArgs.Empty);
            _DisconnectedEventCalled = true;
        }

        /// <summary>
        /// Disconnects the client with the specified reason.
        /// </summary>
        public void Disconnect(MessageID reason)
        {
            Write_Message(reason);
            Disconnect();
        }


        /// <summary>
        /// Calls a Sync Error, Usually when the receiving DataStream contains data the program isn't expecting.
        /// </summary>
        public void SyncError()
        {
            SyncError(new Dictionary<String, Object>());
        }

        public void SyncError(string debugData)
        {
            Dictionary<String, Object> d = new Dictionary<String, Object>();
            d.Add("Debug", debugData);
            SyncError(new Dictionary<String, Object>());
        }

        public void SyncError(Dictionary<String, Object> data)
        {
            StackTrace stackTrace = new StackTrace();

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format("SyncError!"));
                foreach (var kvp in data)
                    System.Diagnostics.Debug.WriteLine(String.Format("{0} = {1}", kvp.Key, kvp.Value));

                System.Diagnostics.Debug.WriteLine("Stack:");
                System.Diagnostics.Debug.WriteLine(stackTrace.ToString());
            }

            Write_Message(MessageID.SyncError);
            Disconnect();
        }

    }
}