using Destructurama.Attributed;
using Newtonsoft.Json;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Requests
{
    public class CreateRealDepositsInvoiceRequest
    {
        [JsonProperty("address")] public string Address { get; set; }
        [JsonProperty("amount")] public int Amount { get; set; }
        [JsonProperty("application_key")] public string ApplicationKey { get; set; }
        [LogMasked(ShowFirst = 1, ShowLast = 1, PreserveLength = true)]

        [JsonProperty("card_exp")] public string CardExp { get; set; }
        [LogMasked(ShowFirst = 6, ShowLast = 4, PreserveLength = true)]

        [JsonProperty("card_number")] public string CardNumber { get; set; }
        [JsonProperty("city")] public string City { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
        [LogMasked(ShowFirst = 0, ShowLast = 0, PreserveLength = true)]
        [JsonProperty("cvv")] public string Cvv { get; set; }
        [JsonProperty("dob")] public string Dob { get; set; }
        [LogMasked(ShowFirst = 3, ShowLast = 3, PreserveLength = true)]
        [JsonProperty("email")] public string Email { get; set; }
        [LogMasked(ShowFirst = 1, ShowLast = 1, PreserveLength = true)]
        [JsonProperty("first_name")] public string FirstName { get; set; }
        [JsonProperty("gateway")] public object Gateway { get; set; }
        [LogMasked(ShowFirst = 1, ShowLast = 1, PreserveLength = true)]
        [JsonProperty("last_name")] public string LastName { get; set; }
        [JsonProperty("merchant_id")] public string MerchantId { get; set; }
        [JsonProperty("notification_url")] public string NotificationUrl { get; set; }
        [JsonProperty("order_id")] public string OrderId { get; set; }
        [LogMasked(ShowFirst = 3, ShowLast = 3, PreserveLength = true)]
        [JsonProperty("phone")] public string Phone { get; set; }
        [JsonProperty("pin")] public string Pin { get; set; }
        [JsonProperty("requester_ip")] public string RequesterIp { get; set; }
        [JsonProperty("return_url")] public string ReturnUrl { get; set; }
        [JsonProperty("state")] public string State { get; set; }
        [JsonProperty("timestamp")] public long Timestamp { get; set; }
        [JsonProperty("transaction_type")] public string TransactionType { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
        [JsonProperty("zip")] public string Zip { get; set; }
        [JsonProperty("signature")] public string Signature { get; set; }
    }
}