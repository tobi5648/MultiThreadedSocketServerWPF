namespace ServerData
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Net; 
    #endregion

    [Serializable]
    public class Packet
    {
        #region Fields
        /// <summary>
        /// The data grouped together.
        /// </summary>
        public List<string> GroupedData;
        /// <summary>
        /// The packets integer.
        /// </summary>
        public int packetInt;
        /// <summary>
        /// The packets boolean.
        /// </summary>
        public bool packetBool;
        /// <summary>
        /// The senders ID.
        /// </summary>
        public string senderID;
        /// <summary>
        /// The type of packet.
        /// </summary>
        public PacketType packetType;
        #endregion

        #region Constructors
        /// <summary>
        /// A constructor for the Packet.
        /// </summary>
        /// <param name="packetType">The type of packet.</param>
        /// <param name="senderID">The ID of the sender.</param>
        public Packet(PacketType packetType, string senderID)
        {
            GroupedData = new List<string>();
            this.senderID = senderID;
            this.packetType = packetType;
        }

        /// <summary>
        /// A constructor for Packet
        /// </summary>
        /// <param name="packetBytes">The bytes of the packet</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        public Packet(byte[] packetBytes)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(packetBytes);
                Packet p = (Packet)bf.Deserialize(ms);
                ms.Close();
                GroupedData = p.GroupedData;
                packetInt = p.packetInt;
                packetBool = p.packetBool;
                senderID = p.senderID;
                packetType = p.packetType;
            }
            catch (ArgumentNullException) { throw; }
            catch (System.Runtime.Serialization.SerializationException) { throw; }
            catch (System.Security.SecurityException) { throw; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gives the data in bytes.
        /// </summary>
        /// <returns>it returns the bytes of data</returns>
        public byte[] ToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }

        /// <summary>
        /// It gives the IP4 Address to communicate.
        /// </summary>
        /// <returns>An Ip 4 Address</returns>
        public static string GetIP4Address()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress i in ips)
            {
                if (i.AddressFamily == AddressFamily.InterNetwork)
                    return i.ToString();
            }
            return "127.0.0.1";
        } 
        #endregion
    }
}
