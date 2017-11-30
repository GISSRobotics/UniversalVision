using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NetworkTables;
using System.Net.Sockets;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NetworkTablesTester1
{
    public sealed partial class MainPage : Page
    {

        /// <summary>
        /// Network Tables reference
        /// </summary>
        private NetworkTables.NetworkTables NT = NetworkTables.NetworkTables.Instance;

        private DispatcherTimer timer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();

            //Socket testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            NT.Initialize("10.64.6.64");

            timer.Interval = new TimeSpan(1000000);
            timer.Tick += Timer_Tick;
            timer.Start();

            NT.SendBool("/SmartDashboard/TesterBool", true);
            NT.SendString("/SmartDashboard/Hello", "World");

            //Test();
        }

        private void Timer_Tick(object sender, object e)
        {
            double value = NT.GetNumber("/SmartDashboard/Tester", -1.0);
            disp.Text = value.ToString();
        }

        public async void Test()
        {
            //IReadOnlyList<Windows.Networking.EndpointPair> eps = await Windows.Networking.Sockets.DatagramSocket.GetEndpointPairsAsync(new Windows.Networking.HostName("google.com"), "0");

            //Windows.Networking.EndpointPair ep = eps.First();

            //System.Net.IPAddress[] hn = await System.Net.Dns.GetHostAddressesAsync("roboRIO-6406-FRC.local");

            //System.Diagnostics.Debug.WriteLine(ep.RemoteHostName);
            //System.Diagnostics.Debug.WriteLine(hn[0]);

            DatagramSocket socketMDns = new DatagramSocket();

            socketMDns.Control.MulticastOnly = true;
            await socketMDns.BindServiceNameAsync("5353");

            socketMDns.MessageReceived += SocketMDns_MessageReceived;

            socketMDns.JoinMulticastGroup(new Windows.Networking.HostName("224.0.0.251"));

            //Windows.Storage.Streams.IOutputStream os = socketMDns.GetOutputStreamAsync()
        }

        private void SocketMDns_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            
        }

        private void sendTest_Click(object sender, RoutedEventArgs e)
        {

            NT.SendNumber("/SmartDashboard/Tester", 4.5);

        }
    }
}
