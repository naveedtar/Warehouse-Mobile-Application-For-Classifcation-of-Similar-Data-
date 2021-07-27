using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Models.InventoryStocks;
using WarehouseHandheld.Services.WebService;

namespace WarehouseHandheld.Services.StockEnquiry
{
    public class StockEnquiryService : IStockEnquiryService
    {
        public WarehouseHandheldService Client { get; private set; }
        public StockEnquiryService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }

        public async Task<InventoryStockSync> GetStockTakesAsync(int productId, string serialNo, int WarehouseId)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.ProductStock).ToString();
                List<string> _queryParameters = new List<string>();
                if (!string.IsNullOrEmpty(serialNo))
                {
                    _queryParameters.Add(string.Format("serialNo={0}", Uri.EscapeDataString(serialNo)));
                }
                _queryParameters.Add(string.Format("productId={0}", Uri.EscapeDataString(productId.ToString())));
                if (WarehouseId != 0)
                    _queryParameters.Add(string.Format("warehouseId={0}", Uri.EscapeDataString(WarehouseId.ToString())));


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

                    return JsonConvert.DeserializeObject<InventoryStockSync>(responseContent);
                }
                return null;
            }
            catch
            {
                return null;
            }

        }
    }
}
