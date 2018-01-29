using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CameraServer.Devices;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace CameraServer.Http
{
    class Server
    {
        private const uint BUFFER_SIZE = 1024;
        private readonly StreamSocketListener _listener;

        private List<Camera> cams;

        public Server()
        {
            _listener = new StreamSocketListener();
            _listener.ConnectionReceived += _listener_ConnectionReceived;
            _listener.Control.KeepAlive = false;
            _listener.Control.NoDelay = false;
            _listener.Control.QualityOfService = SocketQualityOfService.LowLatency;
        }

        public async void Start()
        {
            await _listener.BindServiceNameAsync(80.ToString());
        }

        public void AddCamera(Camera cam)
        {
            cams.Add(cam);
        }

        public void RemoveCamera(int index)
        {
            cams.RemoveAt(index);
        }

        private async void _listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                var socket = args.Socket;

                ServerRequest request = await ReadRequest(socket);
                await WriteResponse(request, socket);

                socket.InputStream.Dispose();
                socket.OutputStream.Dispose();
                socket.Dispose();
            }
            catch { }
        }

        private async Task WriteResponse(ServerRequest request, StreamSocket socket)
        {
            
        }

        private async Task<ServerRequest> ReadRequest(StreamSocket socket)
        {
            string req = string.Empty;
            bool error = false;

            IInputStream inputStream = socket.InputStream;

            byte[] data = new byte[BUFFER_SIZE];
            IBuffer buffer = data.AsBuffer();

            DateTime startReadRequest = DateTime.Now;
            while (!HttpGetRequestHasUrl(req))
            {
                if (DateTime.Now.Subtract(startReadRequest) >= TimeSpan.FromMilliseconds(5000))
                {
                    error = true;
                    return new ServerRequest(null, true);
                }

                var inputStreamReadTask = inputStream.ReadAsync(buffer, BUFFER_SIZE, InputStreamOptions.Partial);
                TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
                await TaskHelper.WithTimeoutAfterStart(ct => inputStreamReadTask.AsTask(ct), timeout);

                req += Encoding.UTF8.GetString(data, 0, (int)inputStreamReadTask.AsTask().Result.Length);
            }

            return new ServerRequest(req, error);
        }

        private bool HttpGetRequestHasUrl(string req)
        {
            var regex = new Regex("GET.*HTTP.*\r\n", RegexOptions.IgnoreCase);
            return regex.IsMatch(req.ToUpper());
        }
    }

    public static class TaskHelper
    {
        public static async Task WithTimeoutAfterStart(Func<CancellationToken, Task> operation, TimeSpan timeout)
        {
            var source = new CancellationTokenSource();
            var task = operation(source.Token);
            source.CancelAfter(timeout);
            await task;
        }
    }
}
