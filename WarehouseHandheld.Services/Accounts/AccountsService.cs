using System;
using System.Threading.Tasks;
using WarehouseHandheld.Services.WebService;
using WarehouseHandheld.Models.Accounts;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace WarehouseHandheld.Services.Accounts
{
    public class AccountsService : IAccountsService
    {
        public WarehouseHandheldService Client { get; private set; }
        public AccountsService(WarehouseHandheldService client)
        {
            if (client == null)
                throw new ArgumentNullException("Client");
            this.Client = client;
        }

        public async Task<AccountSyncCollection> GetAccountsAsync(DateTime dateUpdated, string serialNo)
        {
            try
            {
                var _baseUrl = this.Client.BaseUri.AbsoluteUri;
                var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/", StringComparison.Ordinal) ? "" : "/")), WebServiceConfig.SyncAccounts).ToString();
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

                    return JsonConvert.DeserializeObject<AccountSyncCollection>(responseContent);
                }
                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
