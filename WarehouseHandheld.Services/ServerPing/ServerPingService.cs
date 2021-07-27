using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WarehouseHandheld.Services.WebService;
using WarehouseHandheld.Services.WebService.RestService;

namespace WarehouseHandheld.Services.ServerPing
{
    public class ServerPingService : IServerPingService
    {
        public WarehouseHandheldService Client { get; private set; }
        public ServerPingService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }

        public async Task<ServerStatusEnum> CheckPortConnection(string serialNo)
        {
            //TimeSpan timeout = TimeSpan.FromSeconds(120);
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.PortCheck).ToString();
                List<string> _queryParameters = new List<string>();

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
                using (var httpClient = new HttpClientExtended())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    _httpResponse = await httpClient.SendAsync(_httpRequest).ConfigureAwait(false);
                    if (_httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return ServerStatusEnum.OK;
                    }
                    else if (_httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return ServerStatusEnum.Unauthorized;
                    }
                    return ServerStatusEnum.TimeOut;
                }
               
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return ServerStatusEnum.TimeOut;
            }
            //finally{
            //    this.Client.HttpClient.Timeout = timeout;
            //}
        }


    }

    public enum ServerStatusEnum
    {
        OK = 1,
        TimeOut = 2,
        Unauthorized = 3
    }
}
