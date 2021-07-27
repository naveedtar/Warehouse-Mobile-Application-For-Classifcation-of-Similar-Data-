using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WarehouseHandheld.Services.WebService.RestService
{
    public class HttpClientExtended : System.Net.Http.HttpClient
    {
        public Func<Task<bool>> OnRefreshToken;
        public Func<bool> OnTokenValidation;

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage _httpRequestMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (OnTokenValidation != null && OnRefreshToken != null)
                    if (!OnTokenValidation())
                        await OnRefreshToken().ConfigureAwait(false);

                HttpResponseMessage _httpResponse = await base.SendAsync(_httpRequestMessage, cancellationToken).ConfigureAwait(false);
                HttpStatusCode _statusCode = _httpResponse.StatusCode;
                cancellationToken.ThrowIfCancellationRequested();

                if ((HttpStatusCode)_statusCode == HttpStatusCode.Unauthorized)
                {
                    //refresh token and try again
                    if (OnRefreshToken != null)
                        if (await OnRefreshToken().ConfigureAwait(false))
                            _httpResponse = await base.SendAsync(_httpRequestMessage, cancellationToken).ConfigureAwait(false);
                }

                return _httpResponse;
            }
            catch (Exception ex)
            {

                //if (!CrossConnectivity.Current.IsConnected)
                //{
                //    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                //}

                //if (!await CrossConnectivity.Current.IsReachable(App.WebService.BaseURL, 2000))
                //{
                //    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                //}

                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        public async Task<HttpResponseMessage> BaseSendAsync(HttpRequestMessage _httpRequestMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.SendAsync(_httpRequestMessage, cancellationToken).ConfigureAwait(false);
        }
    }
}
