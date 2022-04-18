using System.Collections.Generic;
using System.Linq;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Requests;
using Finance.PciDss.PciDssBridgeGrpc.Contracts;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Services
{
    public static class RequestValidator
    {
        public static ValidateResult Validate(this CreateRealDepositsInvoiceRequest request)
        {
            var validateResult = new ValidateResult();
            if (request is null)
            {
                validateResult.Add($"{nameof(MakeBridgeDepositGrpcRequest)} is null");
                return validateResult;
            }

            if (string.IsNullOrEmpty(request.Address))
                validateResult.Add($"{nameof(request.Address)} is null or empty");
            if (string.IsNullOrEmpty(request.FirstName))
                validateResult.Add($"{nameof(request.FirstName)} is null or empty");
            if (string.IsNullOrEmpty(request.LastName))
                validateResult.Add($"{nameof(request.LastName)} is null or empty");
            if (string.IsNullOrEmpty(request.CardNumber))
                validateResult.Add($"{nameof(request.CardNumber)} is null or empty");
            if (string.IsNullOrEmpty(request.City)) validateResult.Add($"{nameof(request.City)} is null or empty");
            if (string.IsNullOrEmpty(request.State)) validateResult.Add($"{nameof(request.State)} is null or empty");
            if (string.IsNullOrEmpty(request.Phone)) validateResult.Add($"{nameof(request.Phone)} is null or empty");
            if (string.IsNullOrEmpty(request.Country))
                validateResult.Add($"{nameof(request.Country)} is null or empty");
            if (string.IsNullOrEmpty(request.Email)) validateResult.Add($"{nameof(request.Email)} is null or empty");
            if (string.IsNullOrEmpty(request.Pin)) validateResult.Add($"{nameof(request.Pin)} is null or empty");
            if (string.IsNullOrEmpty(request.Zip)) validateResult.Add($"{nameof(request.Zip)} is null or empty");
            if (string.IsNullOrEmpty(request.RequesterIp))
                validateResult.Add($"{nameof(request.RequesterIp)} is null or empty");

            return validateResult;
        }
    }

    public sealed class ValidateResult
    {
        public IList<string> Errors { get; } = new List<string>();

        public bool IsSuccess => !Errors.Any();

        public bool IsFailed => !IsSuccess;

        public ValidateResult Add(string error)
        {
            Errors.Add(error);
            return this;
        }

        public override string ToString()
        {
            return $"ValidateResult: IsSuccess {IsSuccess}, Errors {string.Join(';', Errors)}";
        }
    }
}
