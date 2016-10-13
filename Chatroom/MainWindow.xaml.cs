using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ServerData;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chatroom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string username;
        public static string id;
        public static Socket master;
        private const int port = 4242;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        public MainWindow(string username)
        {
            this.username = username;
            InitializeComponent();
        A: txtChatBox.Clear();
            txtChatBox.Background = Brushes.DarkRed;
            string ip = Packet.GetIP4Address();
            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            try { master.Connect(ipe); }
            catch
            {
                txtChatBox.Text += "Could not connect to the host!" + Environment.NewLine;
                Thread.Sleep(1000);
                goto A;
            }

            Thread t = new Thread(Data_IN);
            t.Start();
            //for (;;)
            //{
            //    txtChatBox.Text += "::>";
            //    Packet p = new Packet(PacketType.Chat, id);
            //    string input = txtMessageBox.Text;
            //    p.GroupedData.Add(username);
            //    p.GroupedData.Add(input);
            //    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { master.Send(p.ToBytes()); }));
            //    //master.Send(p.ToBytes());
            //}
        }

        private void Data_IN()
        {
            byte[] Buffer;
            int readBytes;
            for (;;)
            {
                try
                {
                    Buffer = new byte[master.SendBufferSize];
                    readBytes = master.Receive(Buffer);
                    if (readBytes > 0)
                    {
                        DataManager(new Packet(Buffer));
                    }
                }
                catch (SocketException)
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { txtChatBox.Text += "The server disconnected!" + Environment.NewLine; }));
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }

        private void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Registration:
                    id = p.GroupedData[0];
                    break;
                case PacketType.Chat:
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { txtChatBox.Foreground = Brushes.Cyan;
                    txtChatBox.AppendText(p.GroupedData[0] + ": " + p.GroupedData[1] + Environment.NewLine);
                    }));
                    break;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            txtChatBox.Text += "::>";
            Packet p = new Packet(PacketType.Chat, id);
            string input = txtMessageBox.Text;
            p.GroupedData.Add(username);
            p.GroupedData.Add(input);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { master.Send(p.ToBytes()); }));
            txtMessageBox.Text = "";
        }
    } 
}
