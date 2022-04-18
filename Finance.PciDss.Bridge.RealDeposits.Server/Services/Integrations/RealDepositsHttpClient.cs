using System.Threading.Tasks;
using Finance.PciDss.Abstractions;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Requests;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Responses;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Serilog;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations
{
    public class RealDepositsHttpClient : IRealDepositsHttpClient
    {
        private readonly ISettingsModelProvider _settingsModelProvider;

        public RealDepositsHttpClient(ISettingsModelProvider settingsModelProvider)
        {
            _settingsModelProvider = settingsModelProvider;
        }

        private SettingsModel SettingsModel => _settingsModelProvider.Get();

        public async Task<Response<CreateRealDepositsInvoiceResponse, string>> RegisterInvoiceAsync(
            CreateRealDepositsInvoiceRequest request)
        {
            Log.Logger.Information("RealDeposits send request : {@requests}, link {link}", request, SettingsModel.RealDepositsApiUrl);
            var result = await SettingsModel
                .RealDepositsApiUrl
                .AppendPathSegments("api", "process")
                .WithHeader("Content-Type", "application/json")
                .AllowHttpStatus("400,422")
                .PostStringAsync(JsonConvert.SerializeObject(request));

            return await result.DeserializeTo<CreateRealDepositsInvoiceResponse, string>();
        }
    }
}