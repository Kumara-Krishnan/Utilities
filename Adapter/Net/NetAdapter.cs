using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities.Adapter.Net.Contract;

namespace Utilities.Adapter.Net
{
    public sealed class NetAdapter : INetAdapter
    {
        private readonly HttpClient HttpClient;

        public NetAdapter()
        {
            HttpClient = new HttpClient();
        }

        public NetAdapter(HttpMessageHandler messageHandler)
        {
            HttpClient = new HttpClient(messageHandler);
        }

        public NetAdapter(IDictionary<string, string> requestHeaders) : this()
        {
            SetDefaultRequestHeaders(requestHeaders);
        }

        public NetAdapter(HttpMessageHandler messageHandler, IDictionary<string, string> requestHeaders) : this(messageHandler)
        {
            SetDefaultRequestHeaders(requestHeaders);
        }

        public void SetDefaultRequestHeaders(IDictionary<string, string> requestHeaders)
        {
            foreach (var requestHeader in requestHeaders)
            {
                HttpClient.DefaultRequestHeaders.Add(requestHeader.Key, requestHeader.Value);
            }
        }

        public Task<string> GetAsync(string uriString)
        {
            return SendAsync(uriString, HttpMethod.Get);
        }

        public Task<string> PostAsync(string uriString, HttpContent content)
        {
            return SendAsync(uriString, HttpMethod.Post, content);
        }

        public Task<string> PostAsync(string uriString, IEnumerable<KeyValuePair<string, string>> requestParameters)
        {
            var content = new FormUrlEncodedContent(requestParameters);
            return PostAsync(uriString, content);
        }

        public async Task<string> SendAsync(string uriString, HttpMethod httpMethod, HttpContent content = default)
        {
            ValidateUri(uriString, out Uri uri);
            var request = new HttpRequestMessage(httpMethod, uri)
            {
                Content = content
            };
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SendAsync(HttpRequestMessage requestMessage)
        {
            var response = await HttpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage requestMessage)
        {
            return HttpClient.SendAsync(requestMessage);
        }

        private void ValidateUri(string uriString, out Uri uri)
        {
            if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
            {
                throw new ArgumentException("Invalid uri");
            }
        }
    }
}
