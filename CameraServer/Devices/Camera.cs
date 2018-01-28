using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Devices.Enumeration;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;

namespace CameraServer.Devices
{
    public class Camera
    {
        private MediaCapture mediaCapture;
        private MediaFrameReader mediaFrameReader;
        private bool isInitialized = false;
        private string pid = string.Empty;

        public MediaCapture Capture
        {
            get
            {
                return mediaCapture;
            }
        }

        public async Task SwitchCameraTo(string pid)
        {
            if (pid == this.pid)
            {
                return;
            }
            await Stop();
            await Initialize(pid);
        }

        public async Task Stop()
        {
            if (isInitialized)
            {
                await mediaFrameReader.StopAsync();
                mediaFrameReader.Dispose();
                await mediaCapture.StopPreviewAsync();
                mediaCapture.Dispose();
                isInitialized = false;
            }
        }

        public async Task Initialize(string pid)
        {
            try
            {
                mediaCapture = new MediaCapture();
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                MediaCaptureInitializationSettings initializationSettings = new MediaCaptureInitializationSettings();
                foreach (var device in devices)
                {
                    System.Diagnostics.Debug.WriteLine(device.Id);
                    if (device.Id.Contains(pid))
                    {
                        initializationSettings.VideoDeviceId = device.Id;
                        this.pid = pid;
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
                isInitialized = true;
            } catch (UnauthorizedAccessException)
            {

            }
        }

    }
}
