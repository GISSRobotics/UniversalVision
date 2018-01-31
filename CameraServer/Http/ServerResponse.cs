using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
