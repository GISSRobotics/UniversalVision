namespace CameraServer.Http
{
    public enum HttpContentType
    {
        Html,
        Javascript,
        Css,
        Mjpeg,
        Text,
        Jpeg
    }

    public static class MimeTypeHelper
    {
        public static string GetHttpContentType(HttpContentType mimeType)
        {
            switch (mimeType)
            {
                case HttpContentType.Html:
                    return "text/html; charset=UTF-8";
                case HttpContentType.Javascript:
                    return "text/javascript; charset-UTF-8";
                case HttpContentType.Css:
                    return "text/css; charset=UTF-8";
                case HttpContentType.Mjpeg:
                    return "multipart/x-mixed-replace;boundary=--boundary";
                case HttpContentType.Text:
                    return "text/plain; charset=UTF-8";
                case HttpContentType.Jpeg:
                    return "image/jpeg";
                default:
                    return "text/plain; charset=UTF-8";
            }
        }
    }
}