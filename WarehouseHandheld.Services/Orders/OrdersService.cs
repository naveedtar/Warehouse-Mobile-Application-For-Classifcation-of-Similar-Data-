using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Services.WebService;
using System.Diagnostics;
using System.Text;
using System.Net.Http.Headers;
using System.Net;

namespace WarehouseHandheld.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private bool _conflictStatus;

        public WarehouseHandheldService Client { get; private set; }
        public OrdersService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }

        public async Task<OrdersSyncCollection> GetOrdersAsync(DateTime dateUpdated, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncOrders).ToString();
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

                    return JsonConvert.DeserializeObject<OrdersSyncCollection>(responseContent);
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        public async Task<OrdersSync> UpdateOrderStatusAsync(string serialNo, int orderId, int statusId, int userId)
        {
            try
            {
                _conflictStatus = false;

                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncOrderStatus).ToString();
                List<string> _queryParameters = new List<string>();

                if (!string.IsNullOrEmpty(serialNo))
                {
                    _queryParameters.Add(string.Format("serialNo={0}", Uri.EscapeDataString(serialNo)));
                }

                _queryParameters.Add(string.Format("orderid={0}", Uri.EscapeDataString(JsonConvert.SerializeObject(orderId))));

                _queryParameters.Add(string.Format("statusid={0}", Uri.EscapeDataString(JsonConvert.SerializeObject(statusId))));

                _queryParameters.Add(string.Format("userid={0}", Uri.EscapeDataString(JsonConvert.SerializeObject(userId))));

                if (_queryParameters.Count > 0)
                {
                    _url += "?" + string.Join("&", _queryParameters);
                }
                HttpRequestMessage _httpRequest = new HttpRequestMessage();
                HttpResponseMessage _httpResponse = null;
                _httpRequest.Method = new HttpMethod("Post");
                _httpRequest.RequestUri = new Uri(_url);

                OrderStatusRequest orderStatusRequest = new OrderStatusRequest()
                {
                    serialNo = serialNo,
                    orderId = orderId,
                    statusId = statusId,
                    userId = userId
                };
                
                string _requestContent = null;
                if (orderStatusRequest != null)
                {
                    _requestContent = JsonConvert.SerializeObject(orderStatusRequest);
                    _httpRequest.Content = new StringContent(_requestContent, Encoding.UTF8);
                    _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                }

                _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest).ConfigureAwait(false);
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = null;
                    responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<OrdersSync>(responseContent);
                }
                if (_httpResponse.StatusCode.Equals(HttpStatusCode.Conflict))
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

        public async Task<OrderReceiveCountSyncCollection> PostReceiveCount(OrderReceiveCountSyncCollection receiveCount)
        {
            try
            {
                _conflictStatus = false;

                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.PostOrderReceiveCount).ToString();

                HttpRequestMessage _httpRequest = new HttpRequestMessage();
                HttpResponseMessage _httpResponse = null;
                _httpRequest.Method = new HttpMethod("Post");
                _httpRequest.RequestUri = new Uri(_url);

                string _requestContent = null;
                if (receiveCount != null)
                {
                    _requestContent = JsonConvert.SerializeObject(receiveCount);
                    _httpRequest.Content = new StringContent(_requestContent, Encoding.UTF8);
                    _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                }

                _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest).ConfigureAwait(false);
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = null;
                    responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<OrderReceiveCountSyncCollection>(responseContent);
                }
                if (_httpResponse.StatusCode.Equals(HttpStatusCode.Conflict))
                {
                    _conflictStatus = true;
                    return null;
                }
                return null;
            }
            catch (Exception e)
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

    class OrderStatusRequest
    {
        public string serialNo { get; set; }
        public int userId { get; set; }
        public int orderId { get; set; }
        public int statusId { get; set; }
    }
}
