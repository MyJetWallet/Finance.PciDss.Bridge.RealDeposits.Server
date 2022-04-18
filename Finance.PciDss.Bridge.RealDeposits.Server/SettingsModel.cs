using SimpleTrading.SettingsReader;

namespace Finance.PciDss.Bridge.RealDeposits.Server
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("PciDssBridgeRealDeposits.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.AuditLogGrpcServiceUrl")]
        public string AuditLogGrpcServiceUrl { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.RealDepositsApiUrl")]
        public string RealDepositsApiUrl { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.RealDepositsRedirectUrl")]
        public string RealDepositsRedirectUrl { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.RealDepositsNotifyUrl")]
        public string RealDepositsNotifyUrl { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.RealDepositsPrivateKey")]
        public string RealDepositsPrivateKey { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.RealDepositsApplicationKey")]
        public string RealDepositsApplicationKey { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.RealDepositsMerchantId")]
        public string RealDepositsMerchantId { get; set; }

        [YamlProperty("PciDssBridgeRealDeposits.RealDepositsGatewayHash")]
        public string GatewayHash { get; set; }
    }
}