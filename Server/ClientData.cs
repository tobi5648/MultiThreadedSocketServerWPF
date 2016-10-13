namespace Server
{
    #region Usings
    using ServerData;
    using System;
    using System.Net.Sockets;
    using System.Threading;
    #endregion

    public class ClientData
    {
        #region Fields
        /// <summary>
        /// The Socket for the client.
        /// </summary>
        public Socket clientSocket;
        /// <summary>
        /// The Thread for the client.
        /// </summary>
        public Thread clientThread;
        /// <summary>
        /// The ID of the client.
        /// </summary>
        public string id; 
        #endregion

        /// <summary>
        /// Constructor of the ClientData.
        /// </summary>
        /// <param name="clientSocket">The Socket it takes.</param>
        public ClientData( Socket clientSocket)
        {
            try
            {
                this.clientSocket = clientSocket;
                id = Guid.NewGuid().ToString();
                clientThread = new Thread(Server.Data_IN);
                clientThread.Start(clientSocket);
                SendRegistrationPacket();
            }
            catch (ArgumentNullException) { throw; }
        }

        /// <summary>
        /// Sends the registration of the server.
        /// </summary>
        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "server");
            p.GroupedData.Add(id);
            clientSocket.Send(p.ToBytes());
        }
    }
}