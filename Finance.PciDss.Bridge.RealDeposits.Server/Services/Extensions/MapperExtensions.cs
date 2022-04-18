using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Finance.PciDss.Abstractions;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Requests;
using Finance.PciDss.PciDssBridgeGrpc;
using Flurl;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Services.Extensions
{
    public static class MapperExtensions
    {
        public static CreateRealDepositsInvoiceRequest ToRealDepositsRestModel(this IPciDssInvoiceModel model, SettingsModel settingsModel)
        {
            var lastName = model.GetLastName();
            var firstName = model.GetName();
            var activityId = Activity.Current?.Id;
            var request = new CreateRealDepositsInvoiceRequest
            {
                Amount = Convert.ToInt32(model.PsAmount * 100),
                Currency = model.PsCurrency,
                Address = model.Address,
                CardNumber = model.CardNumber,
                CardExp = model.ExpirationDate.ToString("MM/yyyy"),
                City = model.City,
                Cvv = model.Cvv,
                Country = model.Country,
                Email = model.Email,
                OrderId = model.OrderId,
                FirstName = firstName,
                LastName = lastName,
                RequesterIp = model.Ip,
                Phone = model.PhoneNumber,
                ReturnUrl = settingsModel.RealDepositsRedirectUrl.SetQueryParam("orderId", model.OrderId).ToString()
                    .SetQueryParam(nameof(activityId), activityId),
                NotificationUrl = settingsModel.RealDepositsNotifyUrl.SetQueryParam(nameof(activityId), activityId),
                State = "none",
                Zip = model.Zip,
                ApplicationKey = settingsModel.RealDepositsApplicationKey,
                Dob = string.Empty,
                Pin = model.TraderId,
                Timestamp = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds(),
                MerchantId = settingsModel.RealDepositsMerchantId,
                TransactionType = "sale",
                Version = "1.2",
                Gateway = settingsModel.GatewayHash
            };

            var signature = request.GenerateSignature(settingsModel);
            request.Signature = signature;
            return request;
        }

        private static Dictionary<string, string> ConvertToDictionary(this CreateRealDepositsInvoiceRequest request)
        {
            return request.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(x=>x.Name)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(request, null)?.ToString());
        }

        public static string GenerateSignature(this CreateRealDepositsInvoiceRequest request, SettingsModel settingsModel)
        {
            var valuesByProperty = request.ConvertToDictionary();
            var values = valuesByProperty.Values.ToList();
            values.Add(settingsModel.RealDepositsPrivateKey);
            var data = string.Join("", values);
            var hash = data.GetSHA384Hash();
            return hash;
        }

        private static string GetSHA384Hash(this string data)
        {
            SHA384 sha384Hash = SHA384.Create();
            byte[] hash = sha384Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            return ByteToString(hash).ToLowerInvariant();
        }

        private static string ByteToString(IEnumerable<byte> buff)
        {
            return buff.Aggregate("", (current, item) => current + item.ToString("X2"));
        }
    }
}
