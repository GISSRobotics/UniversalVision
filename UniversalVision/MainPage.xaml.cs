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
using CameraServer.Devices;
using CameraServer.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalVision
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Network Tables reference
        /// </summary>
        //private NetworkTables.NetworkTables NT = NetworkTables.NetworkTables.Instance;

        private DispatcherTimer timer = new DispatcherTimer();
        
        Camera cam = new Camera();
        Server srv = new Server();


        public MainPage()
        {
            this.InitializeComponent();

            /*
            NT.Initialize(6406);

            timer.Interval = new TimeSpan(1000000);
            timer.Tick += Timer_Tick;
            timer.Start();

            NT.SendBool("/SmartDashboard/TesterBool", true);
            NT.SendString("/SmartDashboard/Hello", "World");
            /**/

            srv.Start();

            StartPreviewAsync();
        }

        private void Timer_Tick(object sender, object e)
        {
            //double value = NT.GetNumber("/SmartDashboard/Tester", -1.0);
           // disp.Text = value.ToString();
        }

        private async void sendTest_Click(object sender, RoutedEventArgs e)
        {

            //NT.SendNumber("/SmartDashboard/Tester", 4.5);
            PreviewControl.Source = null;
            //await cam.SwitchCameraTo("PID_0779");
            try
            {
            }
            catch
            {
                return;
            }

        }

        private async void StartPreviewAsync()
        {
            //string requestedPID = "PID_0779";
            string requestedPID = "PID_0810";
            await cam.Initialize(requestedPID);
            try
            {
                cam.StartProcessing();
                srv.AddCamera(cam);
                //PreviewControl.Source = cam.Capture;
                //await cam.Capture.StartPreviewAsync();
            }
            catch (FileLoadException)
            {
                return;
            }
        }
    }
}
