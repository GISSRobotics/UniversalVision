namespace CameraServer.Http
{
    public enum HttpStatusCode
    {
        HttpCode200,
        HttpCode204,
        HttpCode500
    }

    public static class HttpStatusCodeHelper
    {
        public static string GetHttpStatusCode(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.HttpCode200:
                    return "HTTP/1.1 200 OK";
                case HttpStatusCode.HttpCode204:
                    return "HTTP/1.1 204 No Content";
                case HttpStatusCode.HttpCode500:
                    return "HTTP/1.1 500 Internal Server Error";
                default:
                    return "HTTP/1.1 500 Internal Server Error";
            }
        }
    }
}