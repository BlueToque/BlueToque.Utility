using System.Net;
using System.Net.Http;

namespace BlueToque.Utility
{
    public static class WebHelper
    {
        /// <summary>
        /// Create a web client from the url type
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateClient(string userName = "", string password = "")
        {
            HttpClientHandler handler = (userName.IsNullOrEmpty()) ?
                    new HttpClientHandler() :
                    new HttpClientHandler { Credentials = new NetworkCredential(userName, password) };
            return new HttpClient(handler);
        }

    }
}
