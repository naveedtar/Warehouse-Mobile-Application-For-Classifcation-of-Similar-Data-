using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WarehouseHandheld.Services.Acknowledgements
{
    public interface IAcknowledgementService
    {
        Task<HttpResponseMessage> VerifyAckAsync(string terminalLogId, int count, string serialNo);
    }
}
