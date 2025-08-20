using System.Text.Json;
using ThaiCustomsOcrConsole.Models;
using ThaiCustomsOcrConsole.Services;

namespace ThaiCustomsOcrConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Thai Customs Receipt OCR Parser ===");
            Console.WriteLine();

            // Create the parsing service
            var parsingService = new CustomsReceiptParsingService();

            // Sample OCR text from the requirements
            var sampleOcrText = @"A ) o 7 ~
| ร ใบ เส ร ็ จ ร ั บ เง ิ น
ห ผ่ ก ศก ,. 122
Hawb ฟ ผด. 1746172551 |
ชื ่ อ ย า น พ า ห น ะ 07556 ก ร ม ศุ ด ก า ก ร
เล ข ป ร ะ จ ํ า ต ั ว ผู ้ เส ี ย ภา ษี อ า ก ร 0105533022910/000000 ง ั น ท ี ่ น ํ า เข ้ า / ส ่ ง อ อ ก 25-01-2566 
งื ่ ล ผู้ น ํ า ขอ ง เข ้ า / ผ ู ้ ส ่ ง ขอ ง อ อ ก DHL EXPRESS (โห ล 1 ) LTD.
เล ข ท ี ่ ชํา ร ะ อ า ก ร / ว ั น เด ื อ น ป ี 1818-061169/27-01-66(03/03)
A025-X660100256 (1193)
ค ่ า อ า ก ร ขา เข ้ า - 937.63
ค ํ า ภา ษี ม ู ล ค ่ า เพ ิ ่ ม 65.64
1.00
รวมเงินทั้งสิ้น 1,004.47
น ว น เง ิ น ต ั ว อ ั ก ษ ร หนึ่งพันสี่บาทสี่สิบเจ็ดสตางค์
ล ง ชื อ ผู ้ ร ั บ เง ิ น ว ั ชน ี
( เก ต จ นา )";

            try
            {
                // Parse the OCR text
                Console.WriteLine("Parsing sample Thai customs receipt OCR data...");
                Console.WriteLine();

                var receipt = parsingService.ParseOcrText(sampleOcrText);

                // Serialize to JSON using System.Text.Json
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var jsonResult = JsonSerializer.Serialize(receipt, jsonOptions);

                // Print to console as requested
                Console.WriteLine("=== Parsed CustomsReceipt Object (JSON) ===");
                Console.WriteLine(jsonResult);
                Console.WriteLine();

                // Display some key parsed values
                Console.WriteLine("=== Key Extracted Values ===");
                Console.WriteLine($"Document Type: {receipt.DocumentType}");
                Console.WriteLine($"Organization: {receipt.Organization}");
                Console.WriteLine($"SKC: {receipt.Reference.Skc}");
                Console.WriteLine($"HAWB: {receipt.Reference.Hawb}");
                Console.WriteLine($"Tax ID: {receipt.Reference.TaxId}");
                Console.WriteLine($"Import Duty: {receipt.Items.ImportDuty:C}");
                Console.WriteLine($"VAT: {receipt.Items.Vat:C}");
                Console.WriteLine($"Total Amount: {receipt.Total.Amount:C}");
                Console.WriteLine($"Declarant: {receipt.Reference.DeclarantName}");
                Console.WriteLine();

                Console.WriteLine("=== Demo completed successfully! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            try
            {
                Console.ReadKey();
            }
            catch
            {
                // Handle headless environment
                Console.WriteLine("(Running in headless environment - exiting automatically)");
            }
        }
    }
}
