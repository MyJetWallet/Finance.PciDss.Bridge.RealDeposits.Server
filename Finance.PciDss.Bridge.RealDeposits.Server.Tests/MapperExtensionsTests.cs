using Finance.PciDss.Bridge.RealDeposits.Server.Services.Extensions;
using Finance.PciDss.Bridge.RealDeposits.Server.Services.Integrations.Contracts.Requests;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace Finance.PciDss.Bridge.RealDeposits.Server.Tests
{
    public class Tests
    {
        [Test]
        [Explicit("manual test")]
        public void GenerateSignature_Should_Be_Valid()
        {
            var timestamp = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds();
            var settings = new SettingsModel()
            {
                RealDepositsPrivateKey = "uz39Xj2P6EiLbRl",
                RealDepositsApplicationKey = "handelpro-t",
                RealDepositsMerchantId = "API-handelpro-t",
                GatewayHash = "nsaCowMsdOhjnka2M1z" 
            };
            var request = new CreateRealDepositsInvoiceRequest()
            {
                Address = "Avenue 51/2",
                Amount = 100,
                ApplicationKey = settings.RealDepositsApplicationKey,
                CardExp = "02/2021",
                CardNumber = "5365705373256751",
                City = "New York",
                Country = "US",
                Currency = "USD",
                Cvv = "333",
                Dob = "05/15/1980",
                Email = "test-email@example.com",
                FirstName = "Test",
                Gateway = settings.GatewayHash,
                LastName = "User",
                MerchantId = settings.RealDepositsMerchantId,
                NotificationUrl = "https://webhook.site/4323ad5c-61c0-4fe8-9268-6c232bdfd109",
                OrderId = "test-1560610955",
                Phone = "+1987987987987",
                Pin = "1",
                RequesterIp = "127.0.0.1",
                ReturnUrl = "https://api.merchant.com/v1/deposits/redirect?OrderId=test-1560610955",
                State = "LA", 
                Timestamp = timestamp,
                TransactionType = "sale",
                Version = "1.2",
                Zip = "24123"
            };
            var signature = request.GenerateSignature(settings);
            request.Signature = signature;
            var json = JsonConvert.SerializeObject(request);
            Assert.AreEqual("a24e6e2b4a3255a594c98a519b98b0bae2998b9ee656d043f86cd798af8881094727729a31082f7fffaa1e4c65b4729b", signature);
        }
    }
}