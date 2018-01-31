namespace CameraServer.Http
{
    public enum HttpContentType
    {
        Html,
        Javascript,
        Css,
        Mjpeg
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
                    return "text/plain; charset=UTF-8";
                default:
                    return "text/plain; charset=UTF-8";
            }
        }
    }
}