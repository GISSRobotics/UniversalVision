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

using Windows.Media.Capture;
using Windows.ApplicationModel;
using Windows.System.Display;
using Windows.Graphics.Display;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalVision
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Network Tables reference
        /// </summary>
        private NetworkTables.NetworkTables NT = NetworkTables.NetworkTables.Instance;

        private DispatcherTimer timer = new DispatcherTimer();


        MediaCapture mediaCapture;
        bool isPreviewing;
        DisplayRequest displayRequest = new DisplayRequest();


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

            StartPreviewAsync();
        }

        private void Timer_Tick(object sender, object e)
        {
            double value = NT.GetNumber("/SmartDashboard/Tester", -1.0);
            disp.Text = value.ToString();
        }

        private void sendTest_Click(object sender, RoutedEventArgs e)
        {

            //NT.SendNumber("/SmartDashboard/Tester", 4.5);

        }

        private async void StartPreviewAsync()
        {
            try
            {
                mediaCapture = new MediaCapture();
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                foreach (var device in devices)
                {
                    System.Diagnostics.Debug.WriteLine(device.Name);
                    System.Diagnostics.Debug.WriteLine(device.Id);
                }
                MediaCaptureInitializationSettings initializationSettings = new MediaCaptureInitializationSettings();
                initializationSettings.VideoDeviceId = "\\\\?\\USB#VID_045E&PID_0810&MI_00#6&1e5e39da&0&0000#{e5323777-f976-4f5b-9b55-b94699c46e44}\\GLOBAL";
                await mediaCapture.InitializeAsync(initializationSettings);
                displayRequest.RequestActive();
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            try
            {
                PreviewControl.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
            }
            catch (FileLoadException)
            {
                return;
            }
        }
    }
}
