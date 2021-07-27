using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WarehouseHandheld.Services.WebService;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Services.Vehicles
{
    public class VehiclesService : IVehiclesService
    {
        public WarehouseHandheldService Client { get; private set; }
        public VehiclesService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }

        public async Task<MarketVehiclesSyncCollection> GetVehiclesAsync(DateTime dateUpdated, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncVehicles).ToString();
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

                    return JsonConvert.DeserializeObject<MarketVehiclesSyncCollection>(responseContent);
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<TerminalMetadataSync> GetTerminalMetadataAsync(DateTime dateUpdated, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncTerminalMetaData).ToString();
                List<string> _queryParameters = new List<string>();
                if (dateUpdated != null)
                {
                    _queryParameters.Add(string.Format("reqDate={0}", Uri.EscapeDataString(dateUpdated.ToString("yyyy-MM-dd").Trim('"'))));
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

                    return JsonConvert.DeserializeObject<TerminalMetadataSync>(responseContent);
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
