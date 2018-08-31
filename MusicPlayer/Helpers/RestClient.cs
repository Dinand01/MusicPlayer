using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MusicPlayer.Helpers
{
    /// <summary>
    /// A rest client for communication in the json format.
    /// </summary>
    internal class RestClient : IDisposable
    {
        /// <summary>
        /// The client.
        /// </summary>
        private HttpClient _client;

        /// <summary>
        /// The optional query string.
        /// </summary>
        private string _querystring;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient" /> class.
        /// </summary>
        /// <param name="baseUrl">The base url of the web service.</param>
        /// <param name="queryString">The query string that will be apended on every request.</param>
        public RestClient(string baseUrl, string queryString = null)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _querystring = queryString;
        }

        /// <summary>
        /// Perfroms a GET request.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="endpoint">The endpoint url.</param>
        /// <param name="queryString">The query string for the get request.</param>
        /// <returns>The result.</returns>
        protected async Task<T> Get<T>(string endpoint, string queryString = null)
        {
            string response =  await _client.GetStringAsync(endpoint + GetQueryString(queryString));
            return JsonConvert.DeserializeObject<T>(response);
        }

        /// <summary>
        /// Creates the querystring.
        /// </summary>
        /// <param name="additionalQuerystring">The additional query string.</param>
        /// <returns>The complete query string.</returns>
        private string GetQueryString(string additionalQuerystring = null)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(_querystring) || !string.IsNullOrWhiteSpace(additionalQuerystring))
            {
                result += "?";
            }

            if (!string.IsNullOrWhiteSpace(_querystring))
            {
                result += _querystring.Replace("?", string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(additionalQuerystring))
            {
                result += result.EndsWith("&") || result.EndsWith("?") ? string.Empty : "&";
                result += additionalQuerystring.Replace("?", string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Dispose of the client.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
