using System;
using System.Net.Http;

namespace WebApiContrib.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static bool IsLocal(this HttpRequestMessage request)
        {
            var localFlag = request.Properties["MS_IsLocal"] as Lazy<bool>;
            return localFlag != null && localFlag.Value;
        }
    }
}
