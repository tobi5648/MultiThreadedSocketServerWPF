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

    class Server
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
        static List<ClientData> clients;
        /// <summary>
        /// The IP address.
        /// </summary>
        static IPAddress iPAddress = IPAddress.Parse(Packet.GetIP4Address());
        #endregion

        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server on " + Packet.GetIP4Address());

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(iPAddress, port);
            listenerSocket.Bind(ip);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
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
