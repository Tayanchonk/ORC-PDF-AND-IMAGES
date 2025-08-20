using System.Text.Json.Serialization;

namespace ThaiCustomsOcrApi.Models
{
    public class CustomsReceipt
    {
        [JsonPropertyName("organization")]
        public string Organization { get; set; } = string.Empty;

        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; } = string.Empty;

        [JsonPropertyName("reference")]
        public Reference Reference { get; set; } = new Reference();

        [JsonPropertyName("items")]
        public Items Items { get; set; } = new Items();

        [JsonPropertyName("total")]
        public Total Total { get; set; } = new Total();

        [JsonPropertyName("sign")]
        public Sign Sign { get; set; } = new Sign();
    }

    public class Reference
    {
        [JsonPropertyName("skc")]
        public string Skc { get; set; } = string.Empty;

        [JsonPropertyName("hawb")]
        public string Hawb { get; set; } = string.Empty;

        [JsonPropertyName("vehicleNo")]
        public string VehicleNo { get; set; } = string.Empty;

        [JsonPropertyName("taxId")]
        public string TaxId { get; set; } = string.Empty;

        [JsonPropertyName("importExportDate")]
        public string ImportExportDate { get; set; } = string.Empty;

        [JsonPropertyName("declarantName")]
        public string DeclarantName { get; set; } = string.Empty;

        [JsonPropertyName("declarationNo")]
        public string DeclarationNo { get; set; } = string.Empty;

        [JsonPropertyName("paymentRef")]
        public string PaymentRef { get; set; } = string.Empty;

        [JsonPropertyName("paymentDate")]
        public string PaymentDate { get; set; } = string.Empty;
    }

    public class Items
    {
        [JsonPropertyName("importDuty")]
        public decimal ImportDuty { get; set; }

        [JsonPropertyName("vat")]
        public decimal Vat { get; set; }

        [JsonPropertyName("other")]
        public decimal Other { get; set; }
    }

    public class Total
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("amountText")]
        public string AmountText { get; set; } = string.Empty;
    }

    public class Sign
    {
        [JsonPropertyName("receiver")]
        public string Receiver { get; set; } = string.Empty;

        [JsonPropertyName("officer")]
        public string Officer { get; set; } = string.Empty;
    }
}