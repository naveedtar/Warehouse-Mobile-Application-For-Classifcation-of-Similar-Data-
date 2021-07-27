using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Models.GeoLocation;
using WarehouseHandheld.Services.WebService;

namespace WarehouseHandheld.Services.GeoLocation
{
    public class PostGeoLocationService : IPostGeoLocationService
    {
        public WarehouseHandheldService Client { get; private set; }

        public PostGeoLocationService (WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }
        public async Task<string> PostUserLocationAsync(TerminalGeoLocationViewModel request)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                string _url = "";

                _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.PostGeoLocation).ToString();


                HttpRequestMessage _httpRequest = new HttpRequestMessage();
                HttpResponseMessage _httpResponse = null;
                _httpRequest.Method = new HttpMethod("POST");
                _httpRequest.RequestUri = new Uri(_url);
                string _requestContent = null;
                if (request != null)
                {
                    _requestContent = JsonConvert.SerializeObject(request);
                    _httpRequest.Content = new StringContent(_requestContent, Encoding.UTF8);
                    _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                }
                _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest).ConfigureAwait(false);
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = null;
                    responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return responseContent;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
