namespace Server
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using ServerData;
    using System.Net.Sockets;
    using System.Threading;
    using System.Net; 
    #endregion

    public class Server
    {
        #region Constants
        /// <summary>
        /// The chosen port.
        /// </summary>
        const int port = 4242;
        #endregion
        #region Fields
        /// <summary>
        /// A socket to listen.
        /// </summary>
        static Socket listenerSocket;
        /// <summary>
        /// A list of clients
        /// </summary>
        public static List<ClientData> clients;
        /// <summary>
        /// The IP address.
        /// </summary>
        static IPAddress iPAddress = IPAddress.Parse(Packet.GetIP4Address());

        static bool hasbeen = false;
        #endregion

        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting server on " + Packet.GetIP4Address());

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(iPAddress, port);
            listenerSocket.Bind(ip);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();

            Thread closeThread = new Thread(CloseThread);
            closeThread.Start();
        }

        static void CloseThread()
        {
            for (;;)
            {
                if (hasbeen == true)
                {
                    if (clients.Count == 0)
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        /// <summary>
        /// A method used by a thread to listen for new clients.
        /// </summary>
        static void ListenThread()
        {
            for (;;)
            {
                listenerSocket.Listen(0);
                clients.Add(new ClientData(listenerSocket.Accept()));
                hasbeen = true;
            }
        }

        /// <summary>
        /// Clientdata thread, receives data from each client individually.
        /// </summary>
        /// <param name="cSocket">ClientSocket</param>
        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;
            byte[] Buffer;
            int readBytes;
            for (;;)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer);
                    if (readBytes > 0)
                    {
                        Packet packet = new Packet(Buffer);
                        DataManager(packet);
                    }
                }
                catch (SocketException)
                {
                    try
                    {
                        Console.WriteLine("A client has disconnected");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                    catch (System.Security.SecurityException) { throw; }
                }
                catch (ArgumentNullException) { throw; }
                catch (System.Runtime.Serialization.SerializationException) { throw; }
                catch (System.Security.SecurityException) { throw; }
            }
        }

        /// <summary>
        /// The datamanager, deciding what to do, according to the packet.
        /// </summary>
        /// <param name="p">The packet.</param>
        public static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Chat:
                    foreach (ClientData c in clients)
                        c.clientSocket.Send(p.ToBytes());
                    break;
            }
        }
    }
}
