using Destructurama.Attributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Responses
{
    public class CreateRealDepositsInvoiceResponse
    {
        [JsonProperty("description")] public object Description { get; set; }
        [JsonIgnore()] public string DescriptionString => Description.ToString();
        [JsonProperty("error_code")] public string ErrorCode { get; set; }
        [JsonProperty("error_details")] public string ErrorDetails { get; set; }
        [JsonProperty("payment_processor")] public string PaymentProcessor { get; set; }
        [JsonProperty("redirect_url")] public string RedirectUrl { get; set; }
        [JsonProperty("status")] public int Status { get; set; }
        [JsonProperty("trace_id")] public int TraceId { get; set; }
        [JsonProperty("transaction_id")] public string TransactionId { get; set; }
        [JsonProperty("transaction_status")] public string TransactionStatus { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
        [JsonProperty("signature")] public string Signature { get; set; }

        public bool IsFailed()
        {
            return string.IsNullOrEmpty(TransactionStatus) || TransactionStatus.Equals("rejected", StringComparison.OrdinalIgnoreCase) || Status != 0;
        }

        public bool IsSuccessWithoutRedirectTo3ds()
        {
            return !IsFailed() && TransactionStatus.Equals("approved", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrEmpty(RedirectUrl);
        }

        public bool ShouldBeRedirectTo3ds()
        {
            return !IsFailed()
                && TransactionStatus.Contains("pending", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(RedirectUrl);
        }

        public string GetError()
        {
            return $"Description {Description}, ErrorCode {ErrorCode}, ErrorDetails {ErrorDetails}";
        }
    }
}
