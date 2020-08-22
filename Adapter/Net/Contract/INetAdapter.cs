using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Adapter.Net.Contract
{
    public interface INetAdapter
    {
        void SetDefaultRequestHeaders(IDictionary<string, string> requestHeaders);

        Task<string> GetAsync(string uriString);

        Task<string> PostAsync(string uriString, HttpContent content = default);

        Task<string> PostAsync(string uriString, IEnumerable<KeyValuePair<string, string>> requestParameters);

        Task<string> SendAsync(string uriString, HttpMethod httpMethod, HttpContent content);

        Task<string> SendAsync(HttpRequestMessage requestMessage);

        Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage requestMessage);
    }
}
