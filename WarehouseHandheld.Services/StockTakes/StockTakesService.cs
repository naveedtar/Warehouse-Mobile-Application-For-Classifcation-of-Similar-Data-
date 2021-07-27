using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Models.StockTakes;
using WarehouseHandheld.Services.WebService;

namespace WarehouseHandheld.Services.StockTakes
{
    public class StockTakesService: IStockTakesService
    {
        private bool _conflictStatusScan;
        private bool _conflictStatusUpdate;
        public WarehouseHandheldService Client { get; private set; }
        public StockTakesService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }


        public async Task<StockTakeSyncCollection> GetStockTakesAsync(DateTime dateUpdated, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncStockTakes).ToString();
                List<string> _queryParameters = new List<string>();
                if (dateUpdated != null)
                {
                    _queryParameters.Add(string.Format("reqDate={0}", Uri.EscapeDataString(dateUpdated.ToString("s").Trim('"'))));
                }
                if (!string.IsNullOrEmpty(serialNo))
                {
                    _queryParameters.Add(string.Format("serialNo={0}", Uri.EscapeDataString(serialNo)));
                }
                if (_queryParameters.Count > 0)
                {
                    _url += "?" + string.Join("&", _queryParameters);
                }
                HttpRequestMessage _httpRequest = new HttpRequestMessage();
                HttpResponseMessage _httpResponse = null;
                _httpRequest.Method = new HttpMethod("GET");
                _httpRequest.RequestUri = new Uri(_url);

                _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest).ConfigureAwait(false);
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = null;
                    responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<StockTakeSyncCollection>(responseContent);
                }
                return null;
            }
            catch
            {
                return null;
            }        
        }


        public async Task<StockTakeProductCodeScanResponse> RecordStockScan(StockTakeProductCodeScanRequest request)
        {
            try
            {
                _conflictStatusScan = false;
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.RecordStockScan).ToString();

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

                    return JsonConvert.DeserializeObject<StockTakeProductCodeScanResponse>(responseContent);
                }
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    _conflictStatusScan = true;
                    return null;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public bool HandleStatusConflictScan()
        {
            return _conflictStatusScan;
        }

        public async Task<ResponseObject> UpdateStockQuantity(StockDetailQuantityUpdateRequest request)
        {
            try
            {
                _conflictStatusUpdate = false;
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.UpdateStockQuantity).ToString();

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

                    return JsonConvert.DeserializeObject<ResponseObject>(responseContent);
                }
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    _conflictStatusUpdate = true;
                    return null;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public bool HandleStatusConflictUpdate()
        {
            return _conflictStatusUpdate;
        }
    }
}
