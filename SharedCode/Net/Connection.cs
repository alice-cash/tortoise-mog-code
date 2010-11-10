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
        /// <summary>
        /// The maximum size a packet should be.
        /// There isn't much of a speed advantage if this is increased past an actual
        /// network packets maximum size, but it could increase latency slightly as 
        /// streams of data are not sent and received in 1 packet. If this is larger than 
        /// the OS's buffer, then it can also cause issues due to my faulty logic.
        /// </summary>
        public const int MTU = 1200;

        /// <summary>
        /// We store module information here. Module packets are sent and received via IDs.
        /// This stores that ID and a pointer to a class that handles that data. 
        /// </summary>
        protected static Dictionary<ushort, IComModule> _moduleActions = new Dictionary<ushort, IComModule>();

        /// <summary>
        /// Add a Packet ID and handler to the connection. 
        /// </summary>
        /// <param name="ID">A unique ID that identifies the Module.</param>
        /// <param name="module">A class that contains methods to call when data is received.</param>
        public static void AddModuleHandle(ushort ID, IComModule module)
        {
            //Because the basis of the Module system is on unique IDs, we
            //need to check that it IS unique. Also because its a logic error
            //we throw a generic Exception.
            if (module == null)
                throw new ArgumentNullException("module");
            if (_moduleActions.ContainsKey(ID))
                throw new Exception("ID already exists!");
            _moduleActions.Add(ID, module);
        }

        /// <summary>
        /// This is our buffer for packets. 
        /// </summary>
        /// 

        private SortedDictionary<int, Queue<Packet>> _packetQue;
        //private Queue<Packet> _packetQue = new Queue<Packet>();

        /// <summary>
        /// An event that fires when a core Message is received.
        /// </summary>
        public System.EventHandler<MessageEventArgs> MessageEvent;

        //public static ConnectionState ConnectionState = ConnectionState.NotConnected;

        private bool _readyForData;

        public bool ReadyForData
        {
            get { return _readyForData; }
        }

        /// <summary>
        /// The class we use to read and write to the Socket.
        /// </summary>
        protected NetworkStream _ns;
        /// <summary>
        /// The main socket used for the remote connection.
        /// </summary>
        protected Socket _sock;

        //  protected BinaryReader _br;
        //  protected BinaryWriter _bw;

        //private byte[] _data;

        /// <summary>
        /// Stores the length of the data we need to read on the network, used by
        /// the read function to know if it needs to read a length or read data off
        /// the network.
        /// </summary>
        protected ushort _length;

        //protected DateTime _recived;

        protected byte[] _passKey = null;

        //protected string _authKey;

        public byte[] PassKey
        {
            get { return _passKey; }
        }

        public EventHandler DisconnectedEvent;

        public EventHandler PassKeyRecivedEvent;

        private bool _DisconnectedEventCalled;

        private bool _endRecived;
        private bool _writeRecived;

        //private bool _isasync;


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
            get
            {
                int count = 0;
                foreach (var list in _packetQue)
                    count += list.Value.Count;
                return count;
            }
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
            //_authKey = "";
            _last15Seconds = new LimitedList<ulong>(15, 0);
            _dataRecivedTime = DateTime.Now;
            _packetQue = new SortedDictionary<int, Queue<Packet>>(new PacketSorter());
            // _isasync = false;
            _endRecived = true;
            _writeRecived = false;

            if (_passKey != null)
                WritePasskey(_passKey);
        }


        public void Poll()
        {
            if (!Connected)
            {
                Disconnected();
                return;
            }

            PollRead();
            //Check incase a read kicked us.
            if (!Connected)
            {
                Disconnected();
                return;
            }
            PollWrite();

            if (DateTime.Now - _dataRecivedTime >= TimeSpan.FromSeconds(1))
            {
                _last15Seconds.Add(_dataRecivedCount);
                _lastCount = _dataRecivedCount;
                _dataRecivedCount = 0;
                _dataRecivedTime = DateTime.Now;
            }
        }

        private void PollRead()
        {

            int count = 0;
            //TODO: check for erros!
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
                        break;
                    }
                    PacketID pID;
                    if (!PacketIDHelper.TryParse(pTempID.Result, out pID))
                    {
                        SyncError();
                        break;
                    }

                    if (pID == PacketID.EndRecived)
                    {
                        _endRecived = true;
                    }
                    else if (pID == PacketID.EndRequest)
                    {
                        _writeRecived = true;
                    }
                    else
                    {
                        //remove 2 from the length that we just read from
                        HandleInput(br, Convert.ToUInt16(_length - 2), pID);
                        _dataRecivedCount += 1;
                    }

                    _length = 0;
                }

                count++;
                if (count >= 20)
                    break;

            } while (_length != 0);
        }

        private void PollWrite()
        {
            ExecutionState<Packet> packet, dpacket;
            if (_writeRecived)
            {
                ByteWriter bw = new ByteWriter();
                //2 for ID
                bw.Write(Convert.ToUInt16(2));
                bw.Write(PacketID.EndRecived.Value());
                byte[] data = bw.GetArray();
                WriteData(data, 0, data.Length);
                _writeRecived = false;

            }

            
            if (_endRecived)
               lock (_packetQue)
                    if (_packetQue.Count > 0)
                    {

                        int count = 0;
                        ByteWriter bw = new ByteWriter();
                        int length = 0;
                        while (_packetQue.Count > 0)
                        {

                            packet = PeekPacketFromQue();


                            if (!packet)
                                return;

                            //We throw an Exception here because this is clearly a logic error on our end.
                            //Some @#$%&^* didn't implement something right, somewhere, probably me.
                            if (packet.Result.Data.Length > MTU)
                                throw new Exception("PACKETS MUST BE SMALLER THAN THE MTU!!!!!!!!");

                            if (packet.Result.Data.Length + length < MTU)
                            {
                                //because it it locked, the queue should not have changed.                
                                dpacket = DequeuePacketFromQue();

                                if (!dpacket)
                                    throw new Exception("Logic Error! Peeked packet successfully but failed to dequeue it!");
                                if (!dpacket.Result.Equals(packet.Result))
                                    throw new Exception("Logic Error! Peeked packet different from dequeue packet!");
                                //derp
                            }
                            else
                                break;
                            
                            length += packet.Result.Data.Length;
                            bw.Write(packet.Result.Data);
                            count++;
                        }
                        //2 for ID
                        bw.Write(Convert.ToUInt16(2));
                        bw.Write(PacketID.EndRequest.Value());

                        byte[] data = bw.GetArray();
                        //_ns.BeginWrite(data, 0, data.Length, (IAsyncResult) => { if (IAsyncResult.IsCompleted) _isasync = false; }, null);
                        //_ns.Write(data, 0, data.Length);
                        WriteData(data, 0, data.Length);
                        _endRecived = false;

                    }

        }

        private ExecutionState<Packet> DequeuePacketFromQue()
        {
            lock (_packetQue)
            {
                foreach (var list in _packetQue)
                {
                    if (list.Value.Count > 0)
                        return new ExecutionState<Packet>(true, list.Value.Dequeue());
                }
            }
            return new ExecutionState<Packet>(false, default(Packet));
        }

        private ExecutionState<Packet> PeekPacketFromQue()
        {
            lock (_packetQue)
            {
                foreach (var list in _packetQue)
                {
                    if (list.Value.Count > 0)
                        return new ExecutionState<Packet>(true, list.Value.Peek());
                }
            }
            return new ExecutionState<Packet>(false, default(Packet));
        }





        private void WriteData(byte[] data, int start, int length)
        {
            _ns.Write(data, 0, data.Length);
        }

        private void AddPacket(byte[] data, int priority)
        {
            lock (_packetQue)
            {
                if (_packetQue.ContainsKey(priority))
                {
                    if (_packetQue[priority] == null)
                        _packetQue[priority] = new Queue<Packet>();
                    _packetQue[priority].Enqueue(new Packet(data, priority));
                }
            }
        }



        protected void HandleInput(ByteReader br, ushort length, PacketID pID)
        {

            //Switch through all of the items, even if we need to throw a SyncError.
            //Otherwise each ID should call a Rea{DescritiveInfo}()
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
                    ReadMessage(br);
                    break;
                case PacketID.Key:
                    break;
                case PacketID.ModulePacket:
                    ReadModulePacket(br);
                    break;
            }
        }


        void ReadKey(ByteReader br)
        {
            var tmp = br.ReadString();
            if (!tmp.Sucess)
            {
                SyncError();
                return;
            }
            //finish?
        }

        void ReadMessage(ByteReader br)
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

        void ReadModulePacket(ByteReader br)
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

        private void WritePasskey(byte[] data)
        {
            //2 for ID, x for length
            ushort length = Convert.ToUInt16(2 + data.Length);
            ByteWriter bw = new ByteWriter();

            bw.Write(length);
            bw.Write(PacketID.ModulePacket.Value());
            bw.Write(data);
            AddPacket(bw.GetArray(), Packet.PriorityHighest);
        }

        public void WriteModulePacket(byte[] data, ushort moduleID, int priority = Packet.PriorityNormal)
        {
            //2 for ID, 2 for module ID, x for data length
            ushort length = Convert.ToUInt16(4 + data.Length);
            ByteWriter bw = new ByteWriter();

            bw.Write(length);
            bw.Write(PacketID.ModulePacket.Value());
            bw.Write(moduleID);
            bw.Write(data);
            AddPacket(bw.GetArray(), priority);
        }

        public void WriteMessage(MessageID reason, int priority = Packet.PriorityNormal)
        {
            //2 for ID, 2 for message ID
            ushort length = 4;
            ByteWriter bw = new ByteWriter();

            bw.Write(length);
            bw.Write(PacketID.Message.Value());
            bw.Write(reason.Value());

            AddPacket(bw.GetArray(), priority);
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
            WriteMessage(reason);
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

            WriteMessage(MessageID.SyncError);
            Disconnect();
        }

    }
}