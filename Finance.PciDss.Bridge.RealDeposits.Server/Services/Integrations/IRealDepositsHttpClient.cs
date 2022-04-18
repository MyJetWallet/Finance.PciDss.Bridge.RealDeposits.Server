using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Requests;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Responses;
using System.Threading.Tasks;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations
{
    public interface IRealDepositsHttpClient
    {
        Task<Response<CreateRealDepositsInvoiceResponse, string>> RegisterInvoiceAsync(
            CreateRealDepositsInvoiceRequest request);
    }
}