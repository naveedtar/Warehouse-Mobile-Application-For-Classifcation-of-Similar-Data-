using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseHandheld.Services.WebService;

namespace WarehouseHandheld.Services.Acknowledgements
{
    public class AcknowledgementService : IAcknowledgementService
    {
        public WarehouseHandheldService Client { get; private set; }
        public AcknowledgementService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }


        public async Task<HttpResponseMessage> VerifyAckAsync(string terminalLogId, int count, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.VerifyAck).ToString();
                List<string> _queryParameters = new List<string>();
                if (!string.IsNullOrEmpty(terminalLogId))
                {
                    _queryParameters.Add(string.Format("id={0}", Uri.EscapeDataString(terminalLogId)));
                }
                _queryParameters.Add(string.Format("count={0}", Uri.EscapeDataString(JsonConvert.SerializeObject(count).Trim('"'))));

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
                return _httpResponse;
            }
            catch
            {
                return null;
            }
        }
    }
}
