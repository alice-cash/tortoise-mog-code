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

/*
 * TODO: Impliment Timeout Features on waiting for data or waiting for the remote party to return a Flow Control request.
 * 
 * 
 * */

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

        /// <summary>
        /// Used to verify and identify a connection when connecting to a second connection.
        /// </summary>
        protected byte[] _passKey = null;

        //protected string _authKey;

        /// <summary>
        /// Used to verify and identify a connection when connecting to a second connection
        /// </summary>
        public byte[] PassKey
        {
            get { return _passKey; }
        }

        /// <summary>
        /// Fired when the connects disconnects due to the remote 
        /// </summary>
        public EventHandler DisconnectedEvent;

        /// <summary>
        /// When we receive a key.
        /// </summary>
        public EventHandler<PassKeyRecivedEventArgs> PassKeyRecivedEvent;

        /// <summary>
        /// Keeps track if we have already disconnected.
        /// </summary>
        private bool _DisconnectedEventCalled;

        /// <summary>
        /// Keeps track of when the remote client has told us it received the flow control packet
        /// we sent it. Signaling that we can send more data.
        /// </summary>
        /// 
        private bool _endRecived;
        /// <summary>
        /// Keeps track of when we receive a request to tell the remote party that we received their
        /// flow control packet.
        /// </summary>
        private bool _writeRecived;

        //private bool _isasync;

        /// <summary>
        /// Used to track the time for counting the number of data packets we have received.
        /// </summary>
        private DateTime _dataRecivedTime = DateTime.Now;

        /// <summary>
        /// Used to track the number of data packets we have received in the past second.
        /// </summary>
        private ulong _dataRecivedCount = 0;

        /// <summary>
        /// Used to track the total number of data packets in the previous second.
        /// </summary>
        private ulong _lastCount = 0;

        /// <summary>
        /// Tracks the past 15 seconds worth of packets.
        /// </summary>
        private LimitedList<ulong> _last15Seconds;

        /// <summary>
        /// One second, might not be much of a performance issue, but we run it allot, 
        /// and it will always be the same.
        /// </summary>
        private TimeSpan OneSecondTimespan = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Reusable buffer used to read data off the network.
        /// </summary>
        private byte[] _bf = new byte[MTU];


        /// <summary>
        /// Returns the Average number of packets over the last 15 seconds.
        /// </summary>
        public ulong Last15PacketAverage
        {
            get { return CalculateAverage(_last15Seconds); }
        }

        /// <summary>
        /// Returns the total number of packets over the past 15 seconds.
        /// </summary>
        public ulong Last15PacketTotal
        {
            get
            {
                ulong total = 0;
                foreach (var p in _last15Seconds)
                {
                    total += p;
                }
                return total;
            }
        }

        /// <summary>
        /// Returns the number of packets received in the past second.
        /// </summary>
        public ulong LastPacketSpeed
        {
            get { return _lastCount; }
        }

        /// <summary>
        /// Returns if the socket is connected or not.
        /// </summary>
        public bool Connected
        {
            get { return _sock == null ? false : CheckForError() ? false : _sock.Connected; }
        }

        /// <summary>
        /// Returns the length of the packet queue waiting to send to the remote socket.
        /// </summary>
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

        /// <summary>
        /// Uses an existing Socket to create a Connection class.
        /// </summary>
        /// <param name="connection">A socket that must already be open.</param>
        public Connection(Socket connection)
        {
            _sock = connection;
            Init();
        }

        /// <summary>
        /// Uses an existing Socket to create a Connection class using a passkey.
        /// </summary>
        /// <param name="connection">A socket that must already be open.</param>
        /// <param name="passkey">The passkey to use to connect with.</param>
        public Connection(Socket connection, byte[] passkey)
            : this(connection)
        {
            _passKey = passkey;
        }

        /// <summary>
        /// Creates a Connection class and connects to the remote server.
        /// </summary>
        /// <param name="dest">The IP to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        public Connection(IPAddress dest, int port)
        {
            Connect(dest, port);
            Init();
        }

        /// <summary>
        /// Creates a Connection class and connects to the remote server using a passkey.
        /// </summary>
        /// <param name="dest">The IP to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="passkey">The passkey to use to connect with.</param>
        public Connection(IPAddress dest, int port, byte[] passkey)
            : this(dest, port)
        {
            _passKey = passkey;
        }

        /// <summary>
        /// Creates a Connection class and connects to the remote server.
        /// </summary>
        /// <param name="dest">The IP or Host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        public Connection(string dest, int port)
        {
            IPAddress destAddress;
            if (!IPAddress.TryParse(dest, out destAddress))
            {
                //Maybe its a hostname
                System.Net.IPAddress[] Addresses;

                Addresses = Dns.GetHostEntry(dest).AddressList;

                if (Addresses.Length == 0)
                {
                    throw new Exception("DNS Host did not resolve to an IP address");
                }
                destAddress = Addresses[0];
            }
            Connect(destAddress, port);
            Init();
        }

        /// <summary>
        /// Creates a Connection class and connects to the remote server using a passkey.
        /// </summary>
        /// <param name="dest">The IP or Host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="passkey">The passkey to use to connect with.</param>
        public Connection(string dest, int port, byte[] passkey)
            : this(dest, port)
        {
            _passKey = passkey;
        }


        /// <summary>
        /// Connects to a remote host.
        /// </summary>
        private void Connect(IPAddress dest, int port)
        {
            _sock = new Socket(dest.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sock.Connect(dest, port);
        }

        /// <summary>
        /// Initializes variables used by the class.
        /// </summary>
        private void Init()
        {
            //First we create and set up the network stream, this is what we
            //do all the reading and writing to.
            _ns = new NetworkStream(_sock);
            _DisconnectedEventCalled = false;
            //_authKey = "";
            //We initialize it and it fills with 0s.
            _last15Seconds = new LimitedList<ulong>(15, 0);
            _dataRecivedTime = DateTime.Now;
            //This stores the packets we send. We use the PacketSorter class that
            //sorts lower numbers first.
            _packetQue = new SortedDictionary<int, Queue<Packet>>(new PacketSorter());
            // _isasync = false;
            _endRecived = true;
            _writeRecived = false;

            //If the passkey isn't null, we want to send it out right away.
            if (_passKey != null)
                WritePasskey(_passKey);
        }

        /// <summary>
        /// Polls the Socket, Checking for data and sending out any waiting to get sent.
        /// </summary>
        public void Poll()
        {
            //we don't want any socket errors to travel back up to the caller.
            //This class should function Exception free, though if one does happen,
            //it is most likely due to a module not properly handling issues
            //or a socket error, either way the caller doesn't need to know.
            try
            {
                //the Connected attribute also checks for errors on the socket
                //So either way, we want to make sure the disconnect code is properly run.
                if (!Connected)
                {
                    Disconnected();
                    return;
                }

                PollRead();

                //Check incase a read kicked us or some kind of socket error.
                if (!Connected)
                {
                    Disconnected();
                    return;
                }
                PollWrite();

                //after 1 second, we do our packet count stuff.
                if (DateTime.Now - _dataRecivedTime >= OneSecondTimespan)
                {
                    _last15Seconds.Add(_dataRecivedCount);
                    _lastCount = _dataRecivedCount;
                    _dataRecivedCount = 0;
                    _dataRecivedTime = DateTime.Now;
                }
            }
            catch
            {
                //Any exception is probably due to a socket issue.
                Disconnect();
            }
        }

        /// <summary>
        /// Reads data off the network.
        /// </summary>
        private void PollRead()
        {
            //The number of items we have read off the network.
            //We do this so we can keep track of them the number we have read
            //and we want to limit the number we let the system read at once.
            int count = 0;
            //read the number of bytes available
            //there used to be a race condition in older designs if we just rely
            //on _sock.Available as the socket state can update between here and  
            //later, we can break a bunch of stuff. This may not exist now but i
            //still prefer to guard against it.
            int available = _sock.Available;

            if (available == 0) return;

            //this is our main loop, it allows us to keep reading stuff off the network
            do
            {
                //The core of the network send and receive resoles around sending byte arrays like
                //most games. We prefix a length to the front of these arrays, so if we are not
                //waiting for the rest of some data, we simply look for 2 bytes being available
                //When we are not waiting for data, we check to see if there are 2 bytes.
                if (_length == 0)
                {
                    //If its more than or equal to 2(someone sends 1 byte? that's got to be odd???)
                    if (available >= 2)
                    {
                        //read 2 bytes into the buffer then convert it, so we know what to look for.
                        _ns.Read(_bf, 0, 2);
                        _length = BitConverter.ToUInt16(_bf, 0);

                    }
                }
                //recheck the length
                available = _sock.Available;
                //when we are waiting for data and there's enough
                if (_length > 0 && available >= _length)
                {
                    //we read it and create a byteReader for it,
                    _ns.Read(_bf, 0, _length);
                    ByteReader br = new ByteReader(_bf, 0, _length);

                    //this should succeed, always....
                    //all pieces of data should have a 2 byte ID
                    //if not something bad happened somewhere,
                    var pTempID = br.ReadUShort();
                    if (!pTempID.Sucess)
                    {
                        SyncError();
                        break;
                    }
                    //if its not a valid ID, throw an error as it shouldn't happen.
                    PacketID pID;
                    if (!PacketIDHelper.TryParse(pTempID.Result, out pID))
                    {
                        SyncError();
                        break;
                    }

                    if (pID == PacketID.EndRecived)
                    {
                        //when we receive a response to a flow control packet, we can send more data
                        _endRecived = true;
                    }
                    else if (pID == PacketID.EndRequest)
                    {
                        //we receive a request for flow-control
                        _writeRecived = true;
                    }
                    else
                    {
                        //process the data and increase the packet count.
                        HandleInput(br, pID);
                        _dataRecivedCount += 1;
                    }
                    //we processed the data so reset the length to 0 so we can look for a length again
                    _length = 0;
                }
                available = _sock.Available;

                //increase the counter and stop when we get to 20 processed items
                count++;
                if (count >= 20)
                    break;
                //or until the available data is 0.
            } while (available != 0);
        }

        /// <summary>
        /// Write any available data to the network.
        /// </summary>
        private void PollWrite()
        {
            //Used for storing data from the packet queue
            ExecutionState<Packet> packet, dpacket;
            //If we need to respond to a Flow Control request
            //we simply write it now instead of trying to queue it somewhere else
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

            //If we can send data, then we lock _packetQue so another thread can't 
            //add a packet while we are sending them out. After we lock it, we then check
            //if data is available to send.
            if (_endRecived)
                lock (_packetQue)
                    if (PacketQueCount() > 0)
                    {
                        //We try to fit as many packets into 1 write to speed it up, as writing to the network is a very slow process.

                        int count = 0;
                        ByteWriter bw = new ByteWriter();
                        int length = 0;

                        while (_packetQue.Count > 0)
                        {
                            //There should be a packet in the queue, but we never know what happens.
                            packet = PeekPacketFromQue();

                            //The ExecutionState lets us directly use a Boolean to see if it succeeded or not
                            if (!packet)
                                return;

                            //We throw an Exception here because this is clearly a logic error on our end.
                            //Some @#$%&^* didn't implement something right, somewhere, probably me.
                            if (packet.Result.Data.Length > MTU)
                                throw new Exception("PACKETS MUST BE SMALLER THAN THE MTU!!!!!!!!");

                            //If the packet will fit in and be under the MTU
                            if (packet.Result.Data.Length + length <= MTU)
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
                                //done adding to the queue as it won't fit
                                break;

                            //increase the length and write it to our buffer.
                            length += packet.Result.Data.Length;
                            bw.Write(packet.Result.Data);
                            count++;
                        }


                        byte[] data = bw.GetArray();
                        //_ns.BeginWrite(data, 0, data.Length, (IAsyncResult) => { if (IAsyncResult.IsCompleted) _isasync = false; }, null);
                        //_ns.Write(data, 0, data.Length);

                        //write the data
                        WriteData(data, 0, data.Length);

                        //then a flow control request
                        //2 for ID
                        bw.Write(Convert.ToUInt16(2));
                        bw.Write(PacketID.EndRequest.Value());
                        _endRecived = false;

                    }

        }

        /// <summary>
        /// Simply returns true or false if the socket has an error.
        /// </summary>
        private bool CheckForError()
        {
            return _sock.Poll(10, SelectMode.SelectError);
        }

        /// <summary>
        /// Dequeue a packet from the network queue
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Peek a packet from the network queue
        /// </summary>
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

        /// <summary>
        /// Counts the number of packets in the que.
        /// </summary>
        private int PacketQueCount()
        {
            int count = 0;
            foreach (var list in _packetQue)
                count += list.Value.Count;
            return count;
        }



        /// <summary>
        /// Writes data to the network. Used for profiling and to lazy to remove it.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="start">The start of the array.</param>
        /// <param name="length">The length to write.</param>
        private void WriteData(byte[] data, int start, int length)
        {
            _ns.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Adds a packet to the network queue.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="priority">The priority of the data.</param>
        private void AddPacket(byte[] data, int priority)
        {
            //As this can get called from multiple threads, we must practice safe sex...
            //err um, i mean practice safe cross-threading calls.
            lock (_packetQue)
            {
                //If we don't have this priority in the list, just create it.
                if (!_packetQue.ContainsKey(priority))
                    _packetQue.Add(priority, new Queue<Packet>());
                _packetQue[priority].Enqueue(new Packet(data, priority));
            }
        }


        /// <summary>
        /// Handles input from the PollRead function.
        /// </summary>
        /// <param name="br">The ByteReader with data.</param>
        /// <param name="pID">The ID read off.</param>
        protected void HandleInput(ByteReader br, PacketID pID)
        {

            //Switch through all of the items, even if we need to throw a SyncError.
            //Otherwise each ID should call a Rea{DescritiveInfo}()
            //The reason for the empty SyncError() for a release is we don't care about
            //reasons. We can assume the end developer has
            Dictionary<String, Object> debugData;
            switch (pID)
            {
                //Null is some relic of early testing. No reason to remove it
                case PacketID.Null:
                    debugData = new Dictionary<String, Object>();
                    debugData.Add("PacketID", PacketID.Null);
                    SyncError(debugData);

                    break;
                case PacketID.Message:
                    ReadMessage(br);
                    break;
                case PacketID.Passkey:
                    var result = br.ReadBytes(br.Avaliable);
                    if (!result)
                    {   //???????????
                        Debug.Write("?????????? Could not read Passkey from bytereader");
                        return;
                    }
                    byte[] buffer = result.Result;

                    if (PassKeyRecivedEvent != null)
                        PassKeyRecivedEvent(this, new PassKeyRecivedEventArgs(buffer));

                    break;
                case PacketID.ModulePacket:
                    ReadModulePacket(br);
                    break;
            }
        }

        //Originally was going to do a key for encryption.
        //void ReadKey(ByteReader br)
        //{
        //    var tmp = br.ReadString();
        //    if (!tmp.Sucess)
        //    {
        //        SyncError();
        //        return;
        //    }
        //    //finish?
        //}

        /// <summary>
        /// Reads a message.
        /// </summary>
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
            MessageID mID;
            if (!MessageIDHelper.TryParse(rTmp.Result, out mID))
            {
                Disconnect(MessageID.SyncError);
                return;
            }
            if (MessageEvent != null)
                MessageEvent(this, new MessageEventArgs(mID));
        }

        /// <summary>
        /// Reads a module packet and sends it to the handler.
        /// </summary>
        /// <param name="br"></param>
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
                //throw new Exception("moduleID not registered!");
                return;
            }

            //this directly calls the method the delicate calls. We use the
            //try to catch any bad modules who don't handle errors correctly
            try
            {
                _moduleActions[moduleID.Result].Communication(this, br);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("EXCEPTION! {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Sends the given passkey to the remote connection.
        /// </summary>
        /// <param name="data"></param>
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

        /// <summary>
        /// Sends module data to the remote connection, with the given priority.
        /// </summary>
        /// <param name="data">A byte array of data.</param>
        /// <param name="moduleID">The ID of the remote module.</param>
        /// <param name="priority">The priority of the packet.</param>
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

        /// <summary>
        /// Sends a message to the remote connection.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="priority">The priority of the packet.</param>
        public void WriteMessage(MessageID message, int priority = Packet.PriorityNormal)
        {
            //2 for ID, 2 for message ID
            ushort length = 4;
            ByteWriter bw = new ByteWriter();

            bw.Write(length);
            bw.Write(PacketID.Message.Value());
            bw.Write(message.Value());

            AddPacket(bw.GetArray(), priority);
        }

        /// <summary>
        /// Disconnects the client without any reason.
        /// </summary>
        public void Disconnect()
        {
            if (_sock != null)
            {
                if (_sock.Connected)
                    _sock.Disconnect(false);
                _sock.Close();
            }
            Disconnected();
        }

        /// <summary>
        /// Calculates the average of a list of numbers.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Runs commands after the connection has been closed.
        /// </summary>
        private void Disconnected()
        {
            if (Connected || _DisconnectedEventCalled) return; //UUh, we are not disconnected or this has been called already.
            if (DisconnectedEvent != null)
                DisconnectedEvent(this, EventArgs.Empty);
            _DisconnectedEventCalled = true;
            foreach (var group in _packetQue)
                group.Value.Clear();
            _packetQue.Clear();
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

        /// <summary>
        /// Calls a Sync Error, Usually when the receiving DataStream contains data the program isn't expecting.
        /// </summary>
        /// <param name="debugData">Debug information which is logged.</param>
        public void SyncError(string debugData)
        {
            Dictionary<String, Object> d = new Dictionary<String, Object>();
            d.Add("Debug", debugData);
            SyncError(new Dictionary<String, Object>());
        }

        /// <summary>
        /// Calls a Sync Error, Usually when the receiving DataStream contains data the program isn't expecting.
        /// </summary>
        /// <param name="debugData">Debug information which is logged.</param>
        public void SyncError(Dictionary<String, Object> data)
        {
            StackTrace stackTrace = new StackTrace();

            //If a debugger is attached, chances are we are debugging, so we want to stop and let us
            //and figure out why we got a sync error.
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
            else
            {
                //Otherwise we want to write the data to the Debug Log.
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

    /// <summary>
    /// EventArgs Class used for PassKeyRecivedEvent
    /// </summary>
    public class PassKeyRecivedEventArgs : EventArgs
    {
        /// <summary>
        /// The PassKey Received
        /// </summary>
        public byte[] PassKey;
        /// <summary>
        /// Creates a new instance of the PassKeyRecivedEventArgs class with the given Key.
        /// </summary>
        /// <param name="data">The key received.</param>
        public PassKeyRecivedEventArgs(byte[] data)
        {
            PassKey = data;
        }
    }
}