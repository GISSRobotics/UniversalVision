using CameraServer.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.Storage.Streams;

namespace CameraServer.Http
{
    class ServerResponse
    {
        private static void WriteResponse(HttpContentType? mimeType, byte[] content, HttpStatusCode statusCode, IOutputStream outputStream)
        {
            Stream stream = outputStream.AsStreamForWrite();

            StringBuilder responseHeader = new StringBuilder();
            responseHeader.Append($"{HttpStatusCodeHelper.GetHttpStatusCode(statusCode)}\r\n");

            if (statusCode != HttpStatusCode.HttpCode204)
            {
                responseHeader.Append($"Content-Type: {MimeTypeHelper.GetHttpContentType(mimeType.Value)}\r\n");
                responseHeader.Append($"Content-Length: {content.Length}\r\n");
            }

            responseHeader.Append("Connecction: Close\r\n\r\n");

            var responseHeaderBytes = Encoding.UTF8.GetBytes(responseHeader.ToString());
            stream.Write(responseHeaderBytes, 0, responseHeaderBytes.Length);

            if (content != null)
            {
                stream.Write(content, 0, content.Length);
            }

            stream.Flush();
            stream.Dispose();
        }
        public static void WriteResponseOK(IOutputStream outputStream)
        {
            WriteResponse(null, null, HttpStatusCode.HttpCode204, outputStream);
        }

        public static void WriteResponseText(string text, IOutputStream outputStream)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            WriteResponse(HttpContentType.Text, textBytes, HttpStatusCode.HttpCode200, outputStream);
        }

        public static async void StreamCamera(Camera camera, IOutputStream outputStream)
        {
            Stream stream = outputStream.AsStreamForWrite();

            StringBuilder responseHeader = new StringBuilder();
            responseHeader.Append($"{HttpStatusCodeHelper.GetHttpStatusCode(HttpStatusCode.HttpCode200)}\r\n");
            responseHeader.Append("Cache-Control: no-cache\r\nCache-Control: private\r\n");
            responseHeader.Append($"Content-Type: {MimeTypeHelper.GetHttpContentType(HttpContentType.Mjpeg)}\r\n\r\n");

            System.Diagnostics.Debug.WriteLine(responseHeader.ToString());

            var responseHeaderBytes = Encoding.UTF8.GetBytes(responseHeader.ToString());
            stream.Write(responseHeaderBytes, 0, responseHeaderBytes.Length);

            stream.Flush();

            while (true)
            {
                if (camera.Frame == null)
                {
                    continue;
                }
                byte[] camFrame = new byte[camera.Frame.Count()];
                camera.Frame.CopyTo(camFrame, 0);

                StringBuilder frameHeader = new StringBuilder();
                frameHeader.Append("--boundary\r\n");
                frameHeader.Append($"Content-Type: {MimeTypeHelper.GetHttpContentType(HttpContentType.Jpeg)}\r\n");
                frameHeader.Append($"Content-Length: {camFrame.Length}\r\n\r\n");

                var frameHeaderBytes = Encoding.UTF8.GetBytes(frameHeader.ToString());
                stream.Write(frameHeaderBytes, 0, frameHeaderBytes.Length);

                await stream.WriteAsync(camFrame, 0, camFrame.Length);

                stream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, 2);

                stream.Flush();

            }
        }
    }
}
