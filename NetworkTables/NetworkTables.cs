using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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


        /// <summary>
        /// Class constructor
        /// </summary>
        private NetworkTables()
        {

        }

        /// <summary>
        /// Initialize with a server address
        /// </summary>
        /// <param name="server"></param>
        public void Initialize(string server)
        {
            if (isInitialized)
            {
                throw new Exception("NetworkTables already initialized!");
            }
            serverAddress = server;

            Dns.GetHostEntry(server);

            Socket testSocket = new Socket()

            isInitialized = true;
        }
        
        /// <summary>
        /// Initialize (only once!) with a server address based on a team number: "roboRIO-####-FRC.local"
        /// </summary>
        /// <param name="team"></param>
        public void Initialize(int team)
        {
            string server = String.Format("roboRIO-{0}-FRC.local");
            Initialize(server);
        }

        /// <summary>
        /// Close NetworkTables properly. Initialize can be called after this.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Gets whether we are connected to the remote host
        /// </summary>
        public bool isConnected
        {
            get
            {
                if (testSocket == null)
                {
                    return false;
                }
                return testSocket.Connected;
            }
        }
    }
}
