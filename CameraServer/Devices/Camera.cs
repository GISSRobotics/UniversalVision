using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Devices.Enumeration;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.IO;

namespace CameraServer.Devices
{
    public class Camera
    {
        public byte[] Frame { get; set; }
        private MediaCapture mediaCapture;
        private MediaFrameReader mediaFrameReader;
        private bool isInitialized = false;
        private string pid = string.Empty;
        private bool _stopThreads = false;
        private int _stoppedThreads = 1;

        private volatile object _lastFrameAddedLock = new object();
        private volatile Stopwatch _lastFrameAdded = new Stopwatch();

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

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GarbageCollectorCanWorkHere() { }

        private void ProcessFrames()
        {
            _stoppedThreads--;

            while (!_stopThreads)
            {
                try
                {
                    GarbageCollectorCanWorkHere();

                    MediaFrameReference frame = mediaFrameReader.TryAcquireLatestFrame();

                    Stopwatch frameDuration = new Stopwatch();
                    frameDuration.Start();

                    if (frame == null || frame.VideoMediaFrame == null || frame.VideoMediaFrame.SoftwareBitmap == null)
                    {
                        continue;
                    }

                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        using (var bitmap = SoftwareBitmap.Convert(frame.VideoMediaFrame.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore))
                        {
                            var imageTask = BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream).AsTask();
                            imageTask.Wait();
                            BitmapEncoder encoder = imageTask.Result;
                            encoder.SetSoftwareBitmap(bitmap);

                            var flushTask = encoder.FlushAsync().AsTask();
                            flushTask.Wait();

                            using (var asStream = stream.AsStream())
                            {
                                asStream.Position = 0;

                                var image = new byte[asStream.Length];
                                asStream.Read(image, 0, image.Length);

                                lock (_lastFrameAddedLock)
                                {
                                    if (_lastFrameAdded.Elapsed.Subtract(frameDuration.Elapsed) > TimeSpan.Zero)
                                    {
                                        Frame = image;

                                        _lastFrameAdded = frameDuration;
                                    }
                                }

                                encoder = null;
                            }
                        }
                    }
                }
                catch { }
            }

            _stoppedThreads++;
        }

        public void StartProcessing()
        {
            Task.Factory.StartNew(() =>
            {
                ProcessFrames();
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).AsAsyncAction().AsTask();
        }

        public void StopProcessing()
        {
            _stopThreads = true;
            SpinWait.SpinUntil(() => { return _stoppedThreads == 1; });
            _stopThreads = false;
        }

    }
}
