using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Models.Wastages;
using WarehouseHandheld.Services.WebService;

namespace WarehouseHandheld.Services.Wastages
{
    public class WastagesService : IWastagesService
    {
        private bool _conflictStatus;
        public WarehouseHandheldService Client { get; private set; }
        public WastagesService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }

        public async Task<WastedGoodsReturnResponse> PostWastedGoodsReturn(WastedGoodsReturnRequestSync request)
        {
            try
            {
                _conflictStatus = false;
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.PostWastages).ToString();

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

                    return JsonConvert.DeserializeObject<WastedGoodsReturnResponse>(responseContent);
                }
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    _conflictStatus = true;
                    return null;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public bool HandleStatusConflict()
        {
            return _conflictStatus;
        }
    }
}
