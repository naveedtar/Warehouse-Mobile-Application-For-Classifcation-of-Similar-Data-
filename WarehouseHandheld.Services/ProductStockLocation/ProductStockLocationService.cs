using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.ProductStockLocation;
using WarehouseHandheld.Services.WebService;

namespace WarehouseHandheld.Services.ProductStockLocation
{
    public class ProductStockLocationService : IProductStockLocationService
    {
        public WarehouseHandheldService Client { get; private set; }
        public ProductStockLocationService(WarehouseHandheldService client)
        {
            Client = client ?? throw new ArgumentNullException("client");
        }

        public async Task<ProductLocationStocksSyncCollection> GetAllProductStockLocations(DateTime requestDate, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncProductLocationStocksSync).ToString();
                List<string> _queryParameters = new List<string>();
                if (requestDate != null)
                {
                    _queryParameters.Add(string.Format("reqDate={0}", Uri.EscapeDataString(requestDate.ToString("s").Trim('"'))));
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

                    return JsonConvert.DeserializeObject<ProductLocationStocksSyncCollection>(responseContent);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public interface IProductStockLocationService
    {
        Task<ProductLocationStocksSyncCollection> GetAllProductStockLocations(DateTime requestDate, string serialNo);
    }
}