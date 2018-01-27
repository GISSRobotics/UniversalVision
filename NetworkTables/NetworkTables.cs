using System;
using System.Net.Sockets;
using System.Text;

using System.Diagnostics;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;

namespace NetworkTables
{
    /// <summary>
    /// A singleton class managing all lower-level networking and storage
    /// </summary>
    public sealed class NetworkTables
    {
        private static readonly NetworkTables instance = new NetworkTables();

        /// <summary>
        /// Returns a reference to the singleton instance of this class
        /// </summary>
        public static NetworkTables Instance
        {
            get
            {
                return instance;
            }
        }

        private Socket socket = null;
        private string serverAddress = "";
        private int port = Constants.DEFAULT_PORT;
        private bool isInitialized = false;

        Network mDNS;        

        private Storage store;


        /// <summary>
        /// Class constructor
        /// </summary>
        private NetworkTables()
        {
            mDNS = new Network();
        }

        /// <summary>
        /// Initialize with a server address
        /// </summary>
        /// <param name="server"></param>
        public void Initialize(string server)
        {
            serverAddress = server;
            if (isInitialized)
            {
                throw new Exception("NetworkTables already initialized!");
            }

            Socket testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                testSocket.Connect(serverAddress, port);
            }
            catch (SocketException) {
                // Bad host
                Debug.WriteLine("Failed!");
                return;
            }

            if (!testSocket.Connected)
            {
                return;
            }

            Debug.WriteLine("Success!");

            socket = testSocket;

            store = new Storage();

            isInitialized = true;

            SendHello();

            Task.Run(() => { DispatchLoop(); });
        }
        
        /// <summary>
        /// Initialize (only once!) with a server address based on a team number: "roboRio-####-FRC.local"
        /// </summary>
        /// <param name="team"></param>
        public void Initialize(int team)
        {
            string server = String.Format("roboRIO-{0}-FRC.local", team);
            var info = mDNS.GetInfo(server);
            if (info == null)
            {
                throw new Exception("RoboRio not found!");
            }
            serverAddress = info.Address;
            Initialize(serverAddress);
        }

        public string ResolveMDNS(string name)
        {

            return name;
        }

        /// <summary>
        /// Close NetworkTables properly. Initialize can be called after this.
        /// </summary>
        public void Dispose()
        {
            if (isInitialized)
            {
                socket.Dispose();
            }
        }

        /// <summary>
        /// Gets whether we are connected to the remote host
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (socket == null)
                {
                    return false;
                }
                return socket.Connected;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return this.isInitialized;
            }
        }

        public string ServerAddress
        {
            get
            {
                return serverAddress;
            }
        }

        public bool GetBool(string path, bool fallback)
        {
            if (!IsConnected || !store.HasValue(path))
            {
                return fallback;
            }
            Storage.Item i = store.GetValue(path);
            return Struct.UnpackBool(i.Value);
        }

        public double GetNumber(string path, double fallback)
        {
            if (!IsConnected || !store.HasValue(path))
            {
                return fallback;
            }
            Storage.Item i = store.GetValue(path);
            return Struct.UnpackDouble(Struct.ByteOrder.BigEndian, i.Value);
        }

        public string GetString(string path, string fallback)
        {
            if (!IsConnected || !store.HasValue(path))
            {
                return fallback;
            }
            Storage.Item i = store.GetValue(path);
            return Encoding.UTF8.GetString(i.Value);
        }

        public void SendBool(string path, bool value)
        {
            if (!store.HasValue(path))
            {
                SendAssign(path, Constants.VAR_TYPE.BOOL, Struct.PackBool(value));
            }
            else
            {
                Storage.Item i = store.GetValue(path);
                store.UpdateValue(path, (short)(i.Seq + 1), i.Id, i.Type, Struct.PackBool(value));
                SendUpdate(i.Id, (short)(i.Seq + 1), Struct.PackBool(value));
            }
        }

        public void SendNumber(string path, double value)
        {
            if (!store.HasValue(path))
            {
                SendAssign(path, Constants.VAR_TYPE.NUM, Struct.PackDouble(Struct.ByteOrder.BigEndian, value));
            }
            else
            {
                Storage.Item i = store.GetValue(path);
                store.UpdateValue(path, (short)(i.Seq + 1), i.Id, i.Type, Struct.PackDouble(Struct.ByteOrder.BigEndian, value));
                SendUpdate(i.Id, (short)(i.Seq + 1), Struct.PackDouble(Struct.ByteOrder.BigEndian, value));
            }
        }

        public void SendString(string path, string value)
        {
            byte[] strBuf = Encoding.UTF8.GetBytes(value);
            byte[] buf = new byte[strBuf.Length + 2];
            Struct.PackInt16(Struct.ByteOrder.BigEndian, (short)strBuf.Length).CopyTo(buf, 0);
            strBuf.CopyTo(buf, 2);
            if (!store.HasValue(path))
            {
                SendAssign(path, Constants.VAR_TYPE.STR, buf);
            }
            else
            {
                Storage.Item i = store.GetValue(path);
                store.UpdateValue(path, (short)(i.Seq + 1), i.Id, i.Type, buf);
                SendUpdate(i.Id, (short)(i.Seq + 1), buf);
            }
        }

        private void SendHello()
        {
            Send(new byte[] { 0x01, 0x02, 0x00 });
        }

        private void Send(byte[] data)
        {
            socket.Send(data);
        }

        private byte[] Receive(int length)
        {
            byte[] data = new byte[length];
            if (isInitialized)
            {
                try
                {
                    socket.Receive(data, length, SocketFlags.None);
                }
                catch (SocketException)
                {

                }
            }
            return data;
        }

        private void Process()
        {
            Constants.MSG_TYPE msgType = (Constants.MSG_TYPE)Receive(1)[0];
            switch (msgType)
            {
                case Constants.MSG_TYPE.NOOP:
                    break;
                case Constants.MSG_TYPE.HELLO_DONE:
                    Debug.WriteLine("Hello Complete");
                    break;
                case Constants.MSG_TYPE.ASSIGN:
                    DoAssign();
                    break;
                case Constants.MSG_TYPE.UPDATE:
                    DoUpdate();
                    break;
                default:
                    Debug.WriteLine(String.Format("Unsupported message type: {0}", msgType.ToString("X")));
                    break;
            }
        }

        private void DispatchLoop()
        {
            // Ewww gross! Use a CancellationToken!
            while (true)
            {
                Process();
            }
        }

        private void DoAssign()
        {
            int length = Struct.UnpackInt16(Struct.ByteOrder.BigEndian, Receive(2));
            string path = Encoding.UTF8.GetString(Receive(length));
            Constants.VAR_TYPE varType = (Constants.VAR_TYPE)Receive(1)[0];
            short varId = Struct.UnpackInt16(Struct.ByteOrder.BigEndian, Receive(2));
            short seqNum = Struct.UnpackInt16(Struct.ByteOrder.BigEndian, Receive(2));
            byte[] val = ReadValue(varType);
            store.AddValue(path, varId, varType, val);
        }

        private void SendAssign(string path, Constants.VAR_TYPE type, byte[] value)
        {
            byte[] pathBytes = Encoding.UTF8.GetBytes(path);
            int valueLen = value.Length;
            byte[] msg = new byte[pathBytes.Length + 8 + valueLen];
            msg[0] = (byte)Constants.MSG_TYPE.ASSIGN;
            Struct.PackInt16(Struct.ByteOrder.BigEndian, (short)pathBytes.Length).CopyTo(msg, 1);
            pathBytes.CopyTo(msg, 3);
            msg[pathBytes.Length + 3] = (byte)type;
            (new byte[] { 255, 255, 0, 0 }).CopyTo(msg, pathBytes.Length + 4);
            value.CopyTo(msg, pathBytes.Length + 8);
            Send(msg);
        }

        private void DoUpdate()
        {
            short varId = Struct.UnpackInt16(Struct.ByteOrder.BigEndian, Receive(2));
            short seqNum = Struct.UnpackInt16(Struct.ByteOrder.BigEndian, Receive(2));
            string path = store.GetPathFromId(varId);
            Storage.Item i = store.GetValue(path);
            byte[] val = ReadValue(i.Type);
            if (!store.IsSeqHigher(path, seqNum))
            {
                return;
            }
            store.UpdateValue(path, seqNum, varId, i.Type, val);
        }

        private void SendUpdate(short id, short seq, byte[] value)
        {
            byte[] msg = new byte[5 + value.Length];
            msg[0] = (byte)Constants.MSG_TYPE.UPDATE;
            Struct.PackInt16(Struct.ByteOrder.BigEndian, id).CopyTo(msg, 1);
            Struct.PackInt16(Struct.ByteOrder.BigEndian, seq).CopyTo(msg, 3);
            value.CopyTo(msg, 5);
            Send(msg);
        }

        private byte[] ReadValue(Constants.VAR_TYPE type)
        {
            byte[] val;
            switch (type)
            {
                case Constants.VAR_TYPE.BOOL:
                    val = Readers.ReadBoolAsBytes(Receive);
                    break;
                case Constants.VAR_TYPE.NUM:
                    val = Readers.ReadNumAsBytes(Receive);
                    break;
                case Constants.VAR_TYPE.STR:
                    val = Readers.ReadStrAsBytes(Receive);
                    break;
                case Constants.VAR_TYPE.BOOL_ARRAY:
                    val = new byte[] { 0 };
                    break;
                case Constants.VAR_TYPE.NUM_ARRAY:
                    val = new byte[] { 0 };
                    break;
                case Constants.VAR_TYPE.STR_ARRAY:
                    val = new byte[] { 0 };
                    break;
                default:
                    val = new byte[] { 0 };
                    Debug.WriteLine(String.Format("Unsupported type: {0}", type));
                    break;
            }
            return val;
        }
    }
}
