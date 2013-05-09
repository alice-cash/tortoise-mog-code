///*
// * Copyright 2013 Matthew Cash. All rights reserved.
// * 
// * Redistribution and use in source and binary forms, with or without modification, are
// * permitted provided that the following conditions are met:
// * 
// *    1. Redistributions of source code must retain the above copyright notice, this list of
// *       conditions and the following disclaimer.
// * 
// *    2. Redistributions in binary form must reproduce the above copyright notice, this list
// *       of conditions and the following disclaimer in the documentation and/or other materials
// *       provided with the distribution.
// * 
// * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
// * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
// * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// * 
// * The views and conclusions contained in the software and documentation are those of the
// * authors and should not be interpreted as representing official policies, either expressed
// * or implied, of Matthew Cash.
// */

///*
// * 
// * 
// * 
// * */

using System;
using System.Collections.Generic;
//using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

//using C5;

using Tortoise.Shared.Module;
using Tortoise.Shared.Collection;
using Tortoise.Shared.IO;
using Tortoise.Shared.Net;

namespace Tortoise.Shared.Net
{
    public class Connection
    {
//        /// <summary>
//        /// The maximum size out packet should be. It isn't related to the operating system's MTU
//        /// Increasing it will allow larger packets to be sent out but won't guarantee performance boost.
//        /// Having a missmatch between 2 ends may cause issues.
//        /// </summary>
//        public const int MTU = 1440;

//        #region "Static Variables"
//        /// <summary>
//        /// We store module information here. Module packets are sent and received via IDs.
//        /// This stores that ID and a pointer to a class that handles that data. 
//        /// </summary>
//        protected static Dictionary<ushort, IComModule> _moduleActions = new Dictionary<ushort, IComModule>();
//        #endregion

//        #region "Static Methods"
//        /// <summary>
//        /// Add a Packet ID and handler to the connection. 
//        /// </summary>
//        /// <param name="ID">A unique ID that identifies the Module.</param>
//        /// <param name="module">A class that contains methods to call when data is received.</param>
        public static void AddModuleHandle(ushort ID, IComModule module)
        {
//            //Because the basis of the Module system is on unique IDs, we
//            //need to check that it IS unique. Also because its a logic error
//            //we throw a generic Exception.
//            if (module == null)
//                throw new ArgumentNullException("module");
//            if (_moduleActions.ContainsKey(ID))
//                throw new ArgumentNullException("ID already exists!");
//            _moduleActions.Add(ID, module);
        }
//        #endregion

//        #region private vaiables
//        /// <summary>
//        /// This is our buffer for packets. 
//        /// </summary>
//        /// 
//        private ConcurrentQueue<Packet> _packetQue;

//        /// <summary>
//        /// The class we use to read and write to the Socket.
//        /// </summary>
//        //private NetworkStream _ns;

//        /// <summary>
//        /// The main socket used for the remote connection.
//        /// </summary>
//        private Socket _sock;

//        /// <summary>
//        /// Keep track when we dispose of it.
//        /// </summary>
//        private bool _sockDisposed;

//        /// <summary>
//        /// Stores the length of the data we need to read on the network, used by
//        /// the read function to know if it needs to read a length or read data off
//        /// the network.
//        /// </summary>
//        private ushort _length;

//        /// <summary>
//        /// Keeps track if we have already disconnected.
//        /// </summary>
//        private bool _DisconnectedEventCalled;

//        /// <summary>
//        /// Reusable buffer used to read data off the network.
//        /// </summary>
//        private byte[] _bf = new byte[MTU];

//        /// <summary>
//        /// Used to track the time for counting the number of data packets we have received.
//        /// </summary>
//        private DateTime _dataRecivedTime = DateTime.Now;

//        /// <summary>
//        /// Used to track the number of data packets we have received in the past second.
//        /// </summary>
//        private ulong _dataRecivedCount = 0;

//        /// <summary>
//        /// Used to track the total number of data packets in the previous second.
//        /// </summary>
//        private ulong _lastCount = 0;

//        /// <summary>
//        /// Tracks the past 15 seconds worth of packets.
//        /// </summary>
//        private LimitedList<ulong> _last15Seconds;

//        /// <summary>
//        /// One second, might not be much of a performance issue, but we run it allot, 
//        /// and it will always be the same.
//        /// </summary>
//        private TimeSpan OneSecondTimespan = TimeSpan.FromSeconds(1);

//        #endregion



//        /// <summary>
//        /// Returns if the socket is connected or not. 
//        /// </summary>
        public bool Connected
        {
            get { throw new Exception(); }

//            //Also checks for an error which is useful for the Poll method, 2 birds with 1 stone.
//            get
//            { return _sock == null || _sockDisposed ? false : CheckForError() ? false : _sock.Connected; }
        }

//        /// <summary>
//        /// Returns the length of the packet queue waiting to send to the remote socket.
//        /// </summary>
//        public int PacketQueSize
//        {
//            get
//            {
//                    return _packetQue.Count;
//            }
//        }

        public IPAddress RemoteAddress
        {
            get
            {
                throw new Exception();
            }
//            get
//            {

//                return !Connected ? IPAddress.None : ((IPEndPoint)_sock.RemoteEndPoint).Address;
//            }
        }

//        #region Event Handlers
//        /// <summary>
//        /// An event that fires when a core Message is received.
//        /// </summary>
//        public EventHandler<MessageEventArgs> MessageEvent;

//        /// <summary>
//        /// Fired when the connects disconnects due to the remote 
//        /// </summary>
        public EventHandler DisconnectedEvent;
//        #endregion

//        #region Constructors
//        /// <summary>
//        /// Uses an existing Socket to create a Connection class.
//        /// </summary>
//        /// <param name="connection">A socket that must already be open.</param>
        public Connection(Socket connection)
        {
//            _sock = connection;
//            Init();
        }
//        /// <summary>
//        /// Creates a Connection class and connects to the remote server.
//        /// </summary>
//        /// <param name="dest">The IP to connect to.</param>
//        /// <param name="port">The port to connect to.</param>
        public Connection(IPAddress dest, int port)
        {
//            Connect(dest, port);
//            Init();
        }
//        /// <summary>
//        /// Creates a Connection class and connects to the remote server.
//        /// </summary>
//        /// <param name="dest">The IP or Host to connect to.</param>
//        /// <param name="port">The port to connect to.</param>
        public Connection(string dest, int port)
        {
//            IPAddress destAddress;
//            if (!IPAddress.TryParse(dest, out destAddress))
//            {
//                //Maybe its a hostname
//                System.Net.IPAddress[] Addresses;

//                Addresses = Dns.GetHostEntry(dest).AddressList;

//                if (Addresses.Length == 0)
//                {
//                    throw new Exception("DNS Host did not resolve to an IP address");
//                }
//                //TODO: Impliment sudo load balancing
//                destAddress = Addresses[0];
//            }
//            Connect(destAddress, port);
//            Init();
        }
//        /// <summary>
//        /// Connects to a remote host.
//        /// </summary>
//        private void Connect(IPAddress dest, int port)
//        {
//            _sock = new Socket(dest.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//            _sock.Connect(dest, port);
//        }
//        /// <summary>
//        /// Initializes variables used by the class.
//        /// </summary>
//        private void Init()
//        {
//            _sockDisposed = false;

//            //So that we know the buffer sizes.
//            _sock.ReceiveBufferSize = MTU;
//            _sock.SendBufferSize = MTU;

//            //Our networkstream will do all the heavy lifting.
//            //_ns = new NetworkStream(_sock);
//            _DisconnectedEventCalled = false;

//            //We initialize it and it fills with 0s.
//            _last15Seconds = new LimitedList<ulong>(15, 0);
//            _dataRecivedTime = DateTime.Now;
//            //This stores the packets we send. We use the PacketSorter class that
//            //sorts lower numbers first.
//            _packetQue = new ConcurrentQueue<Packet>();


//        }
//        #endregion



//        /// <summary>
//        /// Polls the Socket, Checking for data and sending out any waiting to get sent.
//        /// </summary>
        public void Poll()
        {
//            //we don't want any socket errors to travel back up to the caller.
//            //This class should function Exception free, though if one does happen,
//            //it is most likely due to a module not properly handling issues
//            //or a socket error, either way the caller doesn't need to know.
//            try
//            {
//                //the Connected attribute also checks for errors on the socket
//                //So either way, we want to make sure the disconnect code is properly run.
//                if (!Connected)
//                {
//                    Disconnected();
//                    return;
//                }

//                PollRead();

//                //Check incase a read kicked us or some kind of socket error.
//                if (!Connected)
//                {
//                    Disconnected();
//                    return;
//                }
//                PollWrite();

//                //after 1 second, we do our packet count stuff.
//                if (DateTime.Now - _dataRecivedTime >= OneSecondTimespan)
//                {
//                    _last15Seconds.Add(_dataRecivedCount);
//                    _lastCount = _dataRecivedCount;
//                    _dataRecivedCount = 0;
//                    _dataRecivedTime = DateTime.Now;
//                }
//            }
//            catch 
//            {
//                //Any exception is probably due to a socket issue.
//                //We don't care about what the exception was
//                Disconnect();
//            }

        }

//        /// <summary>
//        /// Reads data off the network.
//        /// </summary>
//        private void PollRead()
//        {
//            if (_sockDisposed) return;
//            //The number of items we have read off the network.
//            //We do this so we can keep track of them the number we have read
//            //and we want to limit the number we let the system read at once.

//            int count = 0;
//            //read the number of bytes available
//            //there used to be a race condition in older designs if we just rely
//            //on _sock.Available as the socket state can update between here and  
//            //later, we can break a bunch of stuff. This may not exist now but i
//            //still prefer to guard against it.
//            int available = _sock.Available;

//            if (available == 0) return;

//            //this is our main loop, it allows us to keep reading stuff off the network
//            do
//            {
//                //The core of the network send and receive resoles around sending byte arrays like
//                //most games. We prefix a length to the front of these arrays, so if we are not
//                //waiting for the rest of some data, we simply look for 2 bytes being available
//                //When we are not waiting for data, we check to see if there are 2 bytes.
//                if (_length == 0)
//                {
//                    //If its more than or equal to 2(someone sends 1 byte? that's got to be odd???)
//                    if (available >= 2)
//                    {
//                        //read 2 bytes into the buffer then convert it, so we know what to look for.
//                        _sock.Receive(_bf, 0, 2, SocketFlags.None);
//                        _length = BitConverter.ToUInt16(_bf, 0);

//                        if (_length >= MTU - 2)
//                        {
//                            SyncError("Packet Length was larger than MTU!");
//                            return;
//                        }
//                    }
//                }
//                //recheck the length
//                available = _sock.Available;
//                //when we are waiting for data and there's enough
//                if (_length > 0 && available >= _length)
//                {
//                    //we read it and create a byteReader for it,
//                    _sock.Receive(_bf, 0, _length, SocketFlags.None);
//                    ByteReader br = new ByteReader(_bf, 0, _length);

//                    //this should succeed, always....
//                    //all pieces of data should have a 2 byte ID
//                    //if not something bad happened somewhere,
//                    var pTempID = br.ReadUShort();
//                    if (!pTempID.Sucess)
//                    {
//                        SyncError("Malformed Packet, no ID");
//                        break;
//                    }
//                    //if its not a valid ID, throw an error as it shouldn't happen.
//                    PacketID pID;
//                    if (!PacketIDHelper.TryParse(pTempID.Result, out pID))
//                    {
//                        SyncError("Malformed Packet, bad ID");
//                        break;
//                    }

//                    //we processed the data so reset the length to 0 so we can look for a length again
//                    _length = 0;
//                }
//                available = _sock.Available;

//                //increase the counter and stop when we get to 20 processed items
//                count++;
//                if (count >= 20)
//                    break;
//                //or until the available data is 0.
//            } while (available != 0);
//        }

//        /// <summary>
//        /// Write any available data to the network.
//        /// </summary>
//        private void PollWrite()
//        {
//            //Used for storing data from the packet queue
//            ExecutionState<Packet> packet, dpacket;

//            //Check to see if writing will block. If it will then we exit
//            if (WillBlock())
//                return;


//            if (PacketQueCount() > 0)
//            {
//                //We try to fit as many packets into 1 write to speed it up, as writing to the network is a very slow process.

//                int count = 0;
//                ByteWriter bw = new ByteWriter();
//                int length = 0;

//                while (PacketQueCount() > 0)
//                {
//                    //There should be a packet in the queue, but we never know what happens.
//                    packet = PeekPacketFromQue();

//                    //The ExecutionState lets us directly use a Boolean to see if it succeeded or not
//                    if (!packet)
//                        return;

//                    //We throw an Exception here because this is clearly a logic error on our end.
//                    //Some @#$%&^* didn't implement something right, somewhere, probably me.
//                    if (packet.Result.Data.Length > MTU)
//                        throw new Exception("PACKETS MUST BE SMALLER THAN THE MTU!!!!!!!!");

//                    //If the packet will fit in and be under the MTU
//                    if (packet.Result.Data.Length + length <= MTU - 2)
//                    {
//                        //because it it locked, the queue should not have changed.                
//                        dpacket = DequeuePacketFromQue();

//                        if (!dpacket)
//                            throw new Exception("Logic Error! Peeked packet successfully but failed to dequeue it!");
//                        if (!dpacket.Result.Equals(packet.Result))
//                            throw new Exception("Logic Error! Peeked packet different from dequeue packet!");
//                        //derp
//                    }
//                    else
//                        //done adding to the queue as it won't fit
//                        break;

//                    //increase the length and write it to our buffer.
//                    length += packet.Result.Data.Length;
//                    bw.Write(packet.Result.Data);
//                    count++;
//                }



//                byte[] data = bw.GetArray();
//                //_ns.BeginWrite(data, 0, data.Length, (IAsyncResult) => { if (IAsyncResult.IsCompleted) _isasync = false; }, null);
//                //_ns.Write(data, 0, data.Length);

//                //write the data
//                WriteData(data, 0, data.Length);





//            }
//        }


//        /// <summary>
//        /// Writes data to the network. Used for profiling and to lazy to remove it.
//        /// </summary>
//        /// <param name="data">The data to write.</param>
//        /// <param name="start">The start of the array.</param>
//        /// <param name="length">The length to write.</param>
//        private void WriteData(byte[] data, int start, int length)
//        {
//            _sock.Send(data,0,length, SocketFlags.None);
//        }

//        private bool WillBlock()
//        {
//            return !_sock.Poll(0, SelectMode.SelectWrite);
//        }

//        /// <summary>
//        /// Simply returns true or false if the socket has an error.
//        /// </summary>
//        private bool CheckForError()
//        {
//            if (_sockDisposed)
//                return true;
//            return _sock.Poll(0, SelectMode.SelectError);
//        }

//        /// <summary>
//        /// Dequeue a packet from the network queue
//        /// </summary>
//        /// <returns></returns>
//        private ExecutionState<Packet> DequeuePacketFromQue()
//        {
//            Packet result;
//            if (_packetQue.TryDequeue(out result))
//                return new ExecutionState<Packet>(true, result);
//            return new ExecutionState<Packet>(false, default(Packet));
//        }

//        /// <summary>
//        /// Peek a packet from the network queue
//        /// </summary>
//        private ExecutionState<Packet> PeekPacketFromQue()
//        {
//            Packet result;
//            if (_packetQue.TryPeek(out result))
//                return new ExecutionState<Packet>(true, result);
//            return new ExecutionState<Packet>(false, default(Packet));
//        }

//        /// <summary>
//        /// Counts the number of packets in the que.
//        /// </summary>
//        private int PacketQueCount()
//        {
//            return _packetQue.Count;
//        }



//        /// <summary>
//        /// Handles input from the PollRead function.
//        /// </summary>
//        /// <param name="br">The ByteReader with data.</param>
//        /// <param name="pID">The ID read off.</param>
//        protected void HandleInput(ByteReader br, PacketID pID)
//        {

//            //Switch through all of the items, even if we need to throw a SyncError.
//            //Otherwise each ID should call a Rea{DescritiveInfo}()
//            //The reason for the empty SyncError() for a release is we don't care about
//            //reasons. We can assume the end developer has
//            Dictionary<String, Object> debugData;
//            switch (pID)
//            {
//                default:
//                    debugData = new Dictionary<String, Object>();
//                    debugData.Add("PacketID", pID);
//                    SyncError(debugData);
//                    break;
//                case PacketID.Message:
//                    ReadMessage(br);
//                    break;
//                case PacketID.ModulePacket:
//                    ReadModulePacket(br);
//                    break;
//            }
//        }

//        /// <summary>
//        /// Reads a message.
//        /// </summary>
//        void ReadMessage(ByteReader br)
//        {
//            //(MessageID reason)
//            //Check for a valid Enum Item.

//            var rTmp = br.ReadUShort();
//            if (!rTmp.Sucess)
//            {
//                SyncError();
//                return;
//            }
//            MessageID mID;
//            if (!MessageIDHelper.TryParse(rTmp.Result, out mID))
//            {
//                Disconnect(MessageID.SyncError);
//                return;
//            }
//            if (MessageEvent != null)
//                MessageEvent(this, new MessageEventArgs(mID));
//        }

//        /// <summary>
//        /// Reads a module packet and sends it to the handler.
//        /// </summary>
//        /// <param name="br"></param>
//        void ReadModulePacket(ByteReader br)
//        {
//            var moduleID = br.ReadUShort();
//            if (!moduleID.Sucess)
//            {
//                SyncError();
//                return;
//            }
//            if (!_moduleActions.ContainsKey(moduleID.Result))
//            {
//                SyncError();
//                //throw new Exception("moduleID not registered!");
//                return;
//            }

//            //this directly calls the method the delicate calls. We use the
//            //try to catch any bad modules who don't handle errors correctly.
//            try
//            {
//                _moduleActions[moduleID.Result].Communication(this, br);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(string.Format("Module Exception! {0}", ex.ToString()));
//            }

//        }

//        /// <summary>
//        /// Adds a packet to the network queue.
//        /// </summary>
//        /// <param name="data">The data to write.</param>
//        /// <param name="priority">The priority of the data.</param>
//        private void AddPacket(byte[] data)
//        {
//            _packetQue.Enqueue(new Packet(data));
//        }

//        /// <summary>
//        /// Adds a packet to the network queue.
//        /// </summary>
//        /// <param name="data">The data to write.</param>
//        /// <param name="priority">The priority of the data.</param>
//        private void AddPacket(Packet data)
//        {
//            _packetQue.Enqueue(data);
//        }

//        /// <summary>
//        /// Sends module data to the remote connection, with the given priority.
//        /// </summary>
//        /// <param name="data">A byte array of data.</param>
//        /// <param name="moduleID">The ID of the remote module.</param>
//        /// <param name="priority">The priority of the packet.</param>
        public void WriteModulePacket(byte[] data, ushort moduleID)
        {
//            //2 for ID, 2 for module ID, x for data length
//            ushort length = Convert.ToUInt16(4 + data.Length);
//            ByteWriter bw = new ByteWriter();

//            bw.Write(length);
//            bw.Write(PacketID.ModulePacket.Value());
//            bw.Write(moduleID);
//            bw.Write(data);
//            AddPacket(bw.GetArray());
        }

//        /// <summary>
//        /// Sends a message to the remote connection.
//        /// </summary>
//        /// <param name="message">The message.</param>
//        /// <param name="priority">The priority of the packet.</param>
        public void WriteMessage(MessageID message)
        {
//            AddPacket(PackWriteMessage(message));
        }

//                /// <summary>
//        /// Sends a message to the remote connection.
//        /// </summary>
//        /// <param name="message">The message.</param>
//        /// <param name="priority">The priority of the packet.</param>
//        private Packet PackWriteMessage(MessageID message)
//        {
//            //2 for ID, 2 for message ID
//            ushort length = 4;
//            ByteWriter bw = new ByteWriter();

//            bw.Write(length);
//            bw.Write(PacketID.Message.Value());
//            bw.Write(message.Value());

//            return new Packet(bw.GetArray());
//        }

//        /// <summary>
//        /// Disconnects the client without any reason.
//        /// </summary>
        public void Disconnect()
        {
//            if (_sock != null && !_sockDisposed)
//            {
//                if (_sock.Connected)
//                    _sock.Disconnect(false);
//                _sock.Close();
//                _sockDisposed = true;
//            }
//            Disconnected();
        }

//        private void WriteDisconnectReason(MessageID reason)
//        {
//            if (!Connected || _DisconnectedEventCalled) return;
//            Packet message = PackWriteMessage(reason);
//            try
//            {
//                WriteData(message.Data, 0, message.Data.Length);
//            }
//            finally{

//            }

//        }

//        /// <summary>
//        /// Runs commands after the connection has been closed.
//        /// </summary>
//        private void Disconnected()
//        {
//            if (!Connected || _DisconnectedEventCalled) return; //UUh, we are not disconnected or this has been called already.
//            if (DisconnectedEvent != null)
//                DisconnectedEvent(this, EventArgs.Empty);
//            _DisconnectedEventCalled = true;
//        }

//        /// <summary>
//        /// Disconnects the client with the specified reason.
//        /// </summary>
        public void Disconnect(MessageID reason)
        {
//            WriteDisconnectReason(reason);
//            Disconnect();
        }

//        /// <summary>
//        /// Calls a Sync Error, Usually when the receiving DataStream contains data the program isn't expecting.
//        /// </summary>
        public void SyncError()
        {
//            SyncError(new Dictionary<String, Object>());
        }
//        /// <summary>
//        /// Calls a Sync Error, Usually when the receiving DataStream contains data the program isn't expecting.
//        /// </summary>
//        /// <param name="debugData">Debug information which is logged.</param>
        public void SyncError(string debugData)
        {
//            Dictionary<String, Object> d = new Dictionary<String, Object>();
//            d.Add("Debug", debugData);
//            SyncError(new Dictionary<String, Object>());
        }

//        /// <summary>
//        /// Calls a Sync Error, Usually when the receiving DataStream contains data the program isn't expecting.
//        /// </summary>
//        /// <param name="debugData">Debug information which is logged.</param>
        public void SyncError(Dictionary<String, Object> data)
        {
            throw new Exception();
//            StackTrace stackTrace = new StackTrace();

//            //If a debugger is attached, chances are we are debugging, so we want to stop and let us
//            //and figure out why we got a sync error.
//            if (System.Diagnostics.Debugger.IsAttached)
//                System.Diagnostics.Debugger.Break();
//            else
//            {
//                //Otherwise we want to write the data to the Debug Log.
//                System.Diagnostics.Debug.WriteLine(String.Format("SyncError!"));
//                foreach (var kvp in data)
//                    System.Diagnostics.Debug.WriteLine(String.Format("{0} = {1}", kvp.Key, kvp.Value));

//                //STACK TRACES ARE UGLY AND SLOW, WHY ARE WE DOING THIS
//                System.Diagnostics.Debug.WriteLine("Stack:");
//                System.Diagnostics.Debug.WriteLine(stackTrace.ToString());
//            }


//            Disconnect(MessageID.SyncError);
        }
    }

}