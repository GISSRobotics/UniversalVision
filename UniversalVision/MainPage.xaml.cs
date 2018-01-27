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
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;

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


        MediaCapture mediaCapture;
        MediaFrameReader mediaFrameReader;
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
            //double value = NT.GetNumber("/SmartDashboard/Tester", -1.0);
           // disp.Text = value.ToString();
        }

        private void sendTest_Click(object sender, RoutedEventArgs e)
        {

            //NT.SendNumber("/SmartDashboard/Tester", 4.5);

        }

        private async void StartPreviewAsync()
        {
            //string requestedPID = "PID_0779";
            string requestedPID = "PID_0810";
            try
            {
                mediaCapture = new MediaCapture();
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                MediaCaptureInitializationSettings initializationSettings = new MediaCaptureInitializationSettings();
                foreach (var device in devices)
                {
                    if (device.Id.Contains(requestedPID))
                    {
                        initializationSettings.VideoDeviceId = device.Id;
                        break;
                    }
                }
                await mediaCapture.InitializeAsync(initializationSettings);
                MediaFrameSource frameSource = mediaCapture.FrameSources.First().Value;
                VideoDeviceController vdc = frameSource.Controller.VideoDeviceController;
                vdc.DesiredOptimization = MediaCaptureOptimization.LatencyThenQuality;
                vdc.PrimaryUse = CaptureUse.Video;
                mediaCapture.VideoDeviceController.Exposure.TrySetAuto(true);
                var formats = frameSource.SupportedFormats;
                foreach (var format in formats)
                {
                    if (format.VideoFormat.Width == 800 && format.VideoFormat.Width == 600 && Math.Round((double)(format.FrameRate.Numerator / format.FrameRate.Denominator)) == 15)
                    {
                        await frameSource.SetFormatAsync(format);
                        break;
                    }
                }
                mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(frameSource);
                await mediaFrameReader.StartAsync();
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
