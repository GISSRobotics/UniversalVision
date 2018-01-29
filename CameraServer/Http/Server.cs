using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraServer.Devices;
using Windows.Networking.Sockets;

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

                var request = await ReadRequest(socket);
                await WriteResponse(request, socket);

                socket.InputStream.Dispose();
                socket.OutputStream.Dispose();
                socket.Dispose();
            }
            catch { }
        }

        private Task WriteResponse(object request, StreamSocket socket)
        {
            throw new NotImplementedException();
        }

        private Task ReadRequest(StreamSocket socket)
        {
            throw new NotImplementedException();
        }
    }
}
