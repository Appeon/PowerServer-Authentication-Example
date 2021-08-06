using System.Net;
using System.Net.Http;

namespace ServerAPIs.Authentication.Common
{
    public class TokenClient
    {
        public HttpClient Client { get; private set; }

        public TokenClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            httpClient.DefaultRequestVersion = HttpVersion.Version10;

            Client = httpClient;
        }
    }
}
