using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Services.WebService;
using System.Net;
using System.Diagnostics;

namespace WarehouseHandheld.Services.OrderProcesses
{
    public class OrderProcessesService : IOrderProcessesService
    {
        private bool _conflictStatus;
        public WarehouseHandheldService Client { get; private set; }
        public OrderProcessesService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }

        public async Task<OrderProcessesSyncCollection> GetOrderProcessesAsync(DateTime dateUpdated, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncOrderProcesses).ToString();
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

                _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest).ConfigureAwait(true);
                if (_httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string responseContent = null;
                    responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<OrderProcessesSyncCollection>(responseContent);
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<OrdersSync>> PostOrderProcessesAsync(OrderProcessesSyncCollection orderProcessSync)
        {
            try
            {
                _conflictStatus = false;

                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.PostOrderProcesses).ToString();

                HttpRequestMessage _httpRequest = new HttpRequestMessage();
                HttpResponseMessage _httpResponse = null;
                _httpRequest.Method = new HttpMethod("Post");
                _httpRequest.RequestUri = new Uri(_url);

                string _requestContent = null;
                if (orderProcessSync != null)
                {
                    _requestContent = JsonConvert.SerializeObject(orderProcessSync);
                    _httpRequest.Content = new StringContent(_requestContent, Encoding.UTF8);
                    _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                }

                _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest).ConfigureAwait(false);
                if (_httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string responseContent = null;
                    responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<List<OrdersSync>>(responseContent);
                }
                if (_httpResponse.StatusCode.Equals(HttpStatusCode.Conflict))
                {
                    _conflictStatus = true;
                    return null;
                }
                return null;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }
        public bool HandleStatusConflict()
        {
            return _conflictStatus;
        }
    }
}
