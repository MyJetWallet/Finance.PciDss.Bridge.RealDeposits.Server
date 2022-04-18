using System;
using System.Threading.Tasks;
using Finance.PciDss.Abstractions;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Extensions;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations;
using Finance.PciDss.PciDssBridgeGrpc;
using Finance.PciDss.PciDssBridgeGrpc.Contracts;
using Finance.PciDss.PciDssBridgeGrpc.Contracts.Enums;
using Flurl;
using MyCrm.AuditLog.Grpc;
using MyCrm.AuditLog.Grpc.Models;
using Serilog;
using SimpleTrading.Common.Helpers;
using SimpleTrading.GrpcTemplate;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Services
{
    public class RealDepositsGrpcService : IFinancePciDssBridgeGrpcService
    {
        private const string PaymentSystemId = "pciDssRealDepositsBankCards";
        private const string UsdCurrency = "USD";
        private readonly GrpcServiceClient<IMyCrmAuditLogGrpcService> _myCrmAuditLogGrpcService;
        private readonly ISettingsModelProvider _settingsModelProvider;
        private readonly ILogger _logger;
        private readonly IRealDepositsHttpClient _realDepositsHttpClient;

        public RealDepositsGrpcService(IRealDepositsHttpClient realDepositsHttpClient,
            GrpcServiceClient<IMyCrmAuditLogGrpcService> myCrmAuditLogGrpcService,
            ISettingsModelProvider settingsModelProvider,
            ILogger logger)
        {
            _realDepositsHttpClient = realDepositsHttpClient;
            _myCrmAuditLogGrpcService = myCrmAuditLogGrpcService;
            _settingsModelProvider = settingsModelProvider;
            _logger = logger;
        }

        private SettingsModel SettingsModel => _settingsModelProvider.Get();

        public async ValueTask<MakeBridgeDepositGrpcResponse> MakeDepositAsync(MakeBridgeDepositGrpcRequest request)
        {
            _logger.Information("RealDepositsGrpcService start process MakeBridgeDepositGrpcRequest {@request}", request);
            try
            {
                request.PciDssInvoiceGrpcModel.Country = CountryManager.Iso3ToIso2(request.PciDssInvoiceGrpcModel.Country);
                var createRealDepositsInvoiceRequest =
                    request.PciDssInvoiceGrpcModel.ToRealDepositsRestModel(SettingsModel);
                var validateResult = createRealDepositsInvoiceRequest.Validate();
                if (validateResult.IsFailed)
                {
                    _logger.Warning("RealDeposits request is not valid. Errors {@validateResult}", validateResult);
                    await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel,
                        $"Fail RealDeposits create invoice. Error {validateResult}");
                    return MakeBridgeDepositGrpcResponse.Failed(DepositBridgeRequestGrpcStatus.ServerError,
                        validateResult.ToString());
                }

                var response =
                    await _realDepositsHttpClient.RegisterInvoiceAsync(createRealDepositsInvoiceRequest);

                if (response.IsFailed || response.SuccessResult is null || response.SuccessResult.IsFailed())
                {
                    _logger.Information("RealDeposits Fail create invoice. {@response}", response);
                    await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel,
                        $"Fail RealDeposits create invoice. Error {response.FailedResult ?? response.SuccessResult?.GetError()}");
                    return MakeBridgeDepositGrpcResponse.Failed(DepositBridgeRequestGrpcStatus.ServerError,
                        response.FailedResult ?? response.SuccessResult?.GetError());
                }

                if (response.SuccessResult.IsSuccessWithoutRedirectTo3ds())
                {
                    response.SuccessResult.RedirectUrl = SettingsModel.RealDepositsRedirectUrl
                        .SetQueryParam("orderId", request.PciDssInvoiceGrpcModel.OrderId);
                    _logger.Information("RealDeposits is success without redirect to 3ds. RedirectUrl {url} was built for traderId {traderId} and orderid {orderid}", response.SuccessResult.RedirectUrl,
                    request.PciDssInvoiceGrpcModel.TraderId, request.PciDssInvoiceGrpcModel.OrderId);
                    await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel, $"RealDeposits was successful without redirect to 3ds.Orderid: {request.PciDssInvoiceGrpcModel.OrderId}");
                }

                await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel, $"Created deposit invoice with id {request.PciDssInvoiceGrpcModel.OrderId}");
                return MakeBridgeDepositGrpcResponse.Create(response.SuccessResult.RedirectUrl, response.SuccessResult.TransactionId, DepositBridgeRequestGrpcStatus.Success);
            }
            catch (Exception e)
            {
                _logger.Error(e, "RealDepositsGrpcService. MakeDepositAsync failed for traderId {traderId}",
                    request.PciDssInvoiceGrpcModel.TraderId);
                await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel,
                    $"MakeDepositAsync failed for traderId {request.PciDssInvoiceGrpcModel.TraderId}");
                return MakeBridgeDepositGrpcResponse.Failed(DepositBridgeRequestGrpcStatus.ServerError, e.Message);
            }
        }

        public ValueTask<GetPaymentSystemGrpcResponse> GetPaymentSystemNameAsync()
        {
            return new ValueTask<GetPaymentSystemGrpcResponse>(GetPaymentSystemGrpcResponse.Create(PaymentSystemId));
        }

        public ValueTask<GetPaymentSystemCurrencyGrpcResponse> GetPsCurrencyAsync()
        {
            return new ValueTask<GetPaymentSystemCurrencyGrpcResponse>(
                GetPaymentSystemCurrencyGrpcResponse.Create(UsdCurrency));
        }

        public async ValueTask<GetPaymentSystemAmountGrpcResponse> GetPsAmountAsync(GetPaymentSystemAmountGrpcRequest request)
        {
            if (request.Currency.Equals(UsdCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return GetPaymentSystemAmountGrpcResponse.Create(request.Amount, request.Currency);
            }
                
            return default;
        }

        private ValueTask SendMessageToAuditLogAsync(IPciDssInvoiceModel invoice, string message)
        {
            return _myCrmAuditLogGrpcService.Value.SaveAsync(new AuditLogEventGrpcModel
            {
                TraderId = invoice.TraderId,
                Action = "deposit",
                ActionId = invoice.OrderId,
                DateTime = DateTime.UtcNow,
                Message = message
            });
        }
    }
}