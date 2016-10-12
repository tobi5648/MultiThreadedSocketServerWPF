namespace Server
{
    using ServerData;
    #region Usings
    using System;
    using System.Net.Sockets;
    using System.Threading;
    #endregion

    internal class ClientData
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

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "server");
            p.GroupedData.Add(id);
            clientSocket.Send(p.ToBytes());
        }
    }
}