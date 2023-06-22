using System;
using System.Net.Http;

namespace ZeiJakuSei.Modules
{
    internal class HttpClientSingleton
    {
        private static readonly Lazy<HttpClient> lazyInstance = new Lazy<HttpClient>(() => new HttpClient());

        public static HttpClient Instance => lazyInstance.Value;

        private HttpClientSingleton() { }
    }
}
