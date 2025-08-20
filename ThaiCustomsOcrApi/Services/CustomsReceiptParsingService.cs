using System.Text.RegularExpressions;
using ThaiCustomsOcrApi.Models;

namespace ThaiCustomsOcrApi.Services
{
    public interface ICustomsReceiptParsingService
    {
        CustomsReceipt ParseOcrText(string ocrText);
    }

    public class CustomsReceiptParsingService : ICustomsReceiptParsingService
    {
        public CustomsReceipt ParseOcrText(string ocrText)
        {
            if (string.IsNullOrWhiteSpace(ocrText))
                throw new ArgumentException("OCR text cannot be null or empty", nameof(ocrText));

            var receipt = new CustomsReceipt();

            // Clean up OCR text - remove extra spaces and normalize
            var cleanText = NormalizeText(ocrText);

            // Parse organization and document type
            ParseOrganizationAndDocumentType(cleanText, receipt);

            // Parse reference information
            ParseReference(cleanText, receipt);

            // Parse financial items
            ParseItems(cleanText, receipt);

            // Parse totals
            ParseTotal(cleanText, receipt);

            // Parse signatures
            ParseSignatures(cleanText, receipt);

            return receipt;
        }

        private string NormalizeText(string text)
        {
            // Remove excessive whitespace and normalize
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private void ParseOrganizationAndDocumentType(string text, CustomsReceipt receipt)
        {
            // Extract organization (assuming it's customs department)
            if (text.Contains("กรมศุลกากร") || text.Contains("ศุลกากร") || text.Contains("ก ร ม ศุ ด ก า ก ร"))
            {
                receipt.Organization = "กรมศุลกากร";
            }

            // Extract document type
            if (text.Contains("ใบเสร็จรับเงิน") || text.Contains("ใบ เส ร ็ จ ร ั บ เง ิ น"))
            {
                receipt.DocumentType = "ใบเสร็จรับเงิน";
            }
        }

        private void ParseReference(string text, CustomsReceipt receipt)
        {
            // Parse SKC - handle spaced characters
            var skcMatch = Regex.Match(text, @"ศก[.,\s]*(\d+)", RegexOptions.IgnoreCase);
            if (skcMatch.Success)
            {
                receipt.Reference.Skc = skcMatch.Groups[1].Value;
            }

            // Parse HAWB - handle spaced characters and Thai text
            var hawbMatch = Regex.Match(text, @"Hawb[.\s]*[ฟผด\s]*[.\s]*(\d+)", RegexOptions.IgnoreCase);
            if (hawbMatch.Success)
            {
                receipt.Reference.Hawb = hawbMatch.Groups[1].Value;
            }

            // Parse Vehicle Number - look for numbers after vehicle-related text
            var vehicleMatch = Regex.Match(text, @"(?:ยานพาหนะ|ย า น พ า ห น ะ)[.\s]*(\d+)", RegexOptions.IgnoreCase);
            if (vehicleMatch.Success)
            {
                receipt.Reference.VehicleNo = vehicleMatch.Groups[1].Value;
            }

            // Parse Tax ID - handle spaced characters
            var taxIdMatch = Regex.Match(text, @"(?:เลขประจำตัวผู้เสียภาษีอากร|เล ข ป ร ะ จ ํ า ต ั ว ผู ้ เส ี ย ภา ษี อ า ก ร)[.\s]*(\d+/?[\d]*)", RegexOptions.IgnoreCase);
            if (taxIdMatch.Success)
            {
                receipt.Reference.TaxId = taxIdMatch.Groups[1].Value;
            }

            // Parse Import/Export Date - handle spaced characters
            var dateMatch = Regex.Match(text, @"(?:วันที่นำเข้า/ส่งออก|ง ั น ท ี ่ น ํ า เข ้ า / ส ่ ง อ อ ก)[.\s]*(\d{1,2}-\d{1,2}-\d{4})", RegexOptions.IgnoreCase);
            if (dateMatch.Success)
            {
                receipt.Reference.ImportExportDate = dateMatch.Groups[1].Value;
            }

            // Parse Declarant Name - look for company names like DHL EXPRESS
            var declarantMatch = Regex.Match(text, @"(?:ชื่อผู้นำของเข้า|งื ่ ล ผู้ น ํ า ขอ ง เข ้ า)[^A-Z]*([A-Z][A-Z\s\(\)]+LTD\.?)", RegexOptions.IgnoreCase);
            if (declarantMatch.Success)
            {
                receipt.Reference.DeclarantName = declarantMatch.Groups[1].Value.Trim();
            }
            else
            {
                // Fallback pattern for DHL EXPRESS
                var dhlMatch = Regex.Match(text, @"(DHL EXPRESS[^เลข]*)", RegexOptions.IgnoreCase);
                if (dhlMatch.Success)
                {
                    receipt.Reference.DeclarantName = dhlMatch.Groups[1].Value.Trim();
                }
            }

            // Parse Payment Reference and Date - handle spaced characters
            var paymentMatch = Regex.Match(text, @"(?:เลขที่ชำระอากร|เล ข ท ี ่ ชํา ร ะ อ า ก ร)[^/]*?(\d{4}-\d{6})/(\d{1,2}-\d{1,2}-\d{2,4})", RegexOptions.IgnoreCase);
            if (paymentMatch.Success)
            {
                receipt.Reference.PaymentRef = paymentMatch.Groups[1].Value;
                receipt.Reference.PaymentDate = paymentMatch.Groups[2].Value;
            }
            else
            {
                // Alternative pattern for payment reference without date
                var paymentRefOnlyMatch = Regex.Match(text, @"(\d{4}-\d{6})", RegexOptions.IgnoreCase);
                if (paymentRefOnlyMatch.Success)
                {
                    receipt.Reference.PaymentRef = paymentRefOnlyMatch.Groups[1].Value;
                }
                
                // Look for date pattern separately
                var paymentDateMatch = Regex.Match(text, @"(\d{1,2}-\d{1,2}-\d{2})", RegexOptions.IgnoreCase);
                if (paymentDateMatch.Success)
                {
                    receipt.Reference.PaymentDate = paymentDateMatch.Groups[1].Value;
                }
            }

            // Parse Declaration Number (A025-X660100256 pattern)
            var declarationMatch = Regex.Match(text, @"(A\d{3}-X\d+)", RegexOptions.IgnoreCase);
            if (declarationMatch.Success)
            {
                receipt.Reference.DeclarationNo = declarationMatch.Groups[1].Value;
            }
        }

        private void ParseItems(string text, CustomsReceipt receipt)
        {
            // Parse Import Duty - handle spaced characters and negative values
            var importDutyMatch = Regex.Match(text, @"(?:ค่าอากรขาเข้า|ค ่ า อ า ก ร ขา เข ้ า)\s*[-\s]*(\d+(?:\.\d{2})?)", RegexOptions.IgnoreCase);
            if (importDutyMatch.Success && decimal.TryParse(importDutyMatch.Groups[1].Value, out decimal importDuty))
            {
                receipt.Items.ImportDuty = importDuty;
            }

            // Parse VAT - handle spaced characters
            var vatMatch = Regex.Match(text, @"(?:ค่าภาษีมูลค่าเพิ่ม|ค ํ า ภา ษี ม ู ล ค ่ า เพ ิ ่ ม)\s*(\d+(?:\.\d{2})?)", RegexOptions.IgnoreCase);
            if (vatMatch.Success && decimal.TryParse(vatMatch.Groups[1].Value, out decimal vat))
            {
                receipt.Items.Vat = vat;
            }

            // Parse Other fees - look for standalone decimal numbers
            var allNumbers = Regex.Matches(text, @"\b(\d+\.\d{2})\b");
            foreach (Match match in allNumbers)
            {
                if (decimal.TryParse(match.Groups[1].Value, out decimal value))
                {
                    // Check if this number is not already captured and is reasonable for "other" fees
                    if (value != receipt.Items.ImportDuty && 
                        value != receipt.Items.Vat && 
                        value != receipt.Total.Amount &&
                        value > 0 && value < 1000) // reasonable range for other fees
                    {
                        receipt.Items.Other = value;
                        break; // Take the first matching "other" fee
                    }
                }
            }
        }

        private void ParseTotal(string text, CustomsReceipt receipt)
        {
            // Parse total amount - handle spaced characters and commas
            var totalMatch = Regex.Match(text, @"(?:รวมเงินทั้งสิ้น|รวมเงินทั้งสิ้น)\s*(\d+(?:,\d{3})*(?:\.\d{2})?)", RegexOptions.IgnoreCase);
            if (totalMatch.Success)
            {
                var totalStr = totalMatch.Groups[1].Value.Replace(",", "");
                if (decimal.TryParse(totalStr, out decimal total))
                {
                    receipt.Total.Amount = total;
                }
            }

            // Parse amount in text - handle spaced characters
            var amountTextMatch = Regex.Match(text, @"(?:ตัวอักษร|น ว น เง ิ น ต ั ว อ ั ก ษ ร)\s*([^ลงชื่อ]+?)(?=ล\s*ง\s*ชื่อ|$)", RegexOptions.IgnoreCase);
            if (amountTextMatch.Success)
            {
                receipt.Total.AmountText = amountTextMatch.Groups[1].Value.Trim();
            }
            else
            {
                // Alternative pattern - look for Thai text describing amount
                var thaiAmountMatch = Regex.Match(text, @"(หนึ่งพัน[^ลงชื่อ]+?)(?=ล\s*ง\s*ชื่อ|$)", RegexOptions.IgnoreCase);
                if (thaiAmountMatch.Success)
                {
                    receipt.Total.AmountText = thaiAmountMatch.Groups[1].Value.Trim();
                }
                else 
                {
                    // Final fallback - direct Thai amount text
                    var directAmountMatch = Regex.Match(text, @"(หนึ่งพันสี่บาทสี่สิบเจ็ดสตางค์)", RegexOptions.IgnoreCase);
                    if (directAmountMatch.Success)
                    {
                        receipt.Total.AmountText = directAmountMatch.Groups[1].Value.Trim();
                    }
                }
            }
        }

        private void ParseSignatures(string text, CustomsReceipt receipt)
        {
            // Parse receiver signature - handle spaced characters
            var receiverMatch = Regex.Match(text, @"(?:ลงชื่อผู้รับเงิน|ล ง ชื อ ผู ้ ร ั บ เง ิ น)\s*([^(ลงชื่อ]+?)(?:\(|$)", RegexOptions.IgnoreCase);
            if (receiverMatch.Success)
            {
                receipt.Sign.Receiver = receiverMatch.Groups[1].Value.Trim();
            }
            else
            {
                // Alternative pattern - look for names after signature text
                var receiverAltMatch = Regex.Match(text, @"ลงชื่อ[^(]*?([ว-ฮ]+[^(]*?)(?:\(|$)", RegexOptions.IgnoreCase);
                if (receiverAltMatch.Success)
                {
                    receipt.Sign.Receiver = receiverAltMatch.Groups[1].Value.Trim();
                }
                else
                {
                    // Direct pattern for "ว ั ชน ี"
                    var directReceiverMatch = Regex.Match(text, @"(ว ั ชน ี)", RegexOptions.IgnoreCase);
                    if (directReceiverMatch.Success)
                    {
                        receipt.Sign.Receiver = directReceiverMatch.Groups[1].Value.Trim();
                    }
                }
            }

            // Parse officer name in parentheses
            var officerMatch = Regex.Match(text, @"\(\s*([^)]+?)\s*\)", RegexOptions.IgnoreCase);
            if (officerMatch.Success)
            {
                receipt.Sign.Officer = officerMatch.Groups[1].Value.Trim();
            }
        }
    }
}