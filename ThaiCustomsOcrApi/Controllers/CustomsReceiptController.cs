using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ThaiCustomsOcrApi.Models;
using ThaiCustomsOcrApi.Services;

namespace ThaiCustomsOcrApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomsReceiptController : ControllerBase
    {
        private readonly ICustomsReceiptParsingService _parsingService;
        private readonly ILogger<CustomsReceiptController> _logger;

        public CustomsReceiptController(ICustomsReceiptParsingService parsingService, ILogger<CustomsReceiptController> logger)
        {
            _parsingService = parsingService;
            _logger = logger;
        }

        /// <summary>
        /// Parse OCR text from Thai customs receipt and return structured data
        /// </summary>
        /// <param name="request">OCR text to parse</param>
        /// <returns>Structured customs receipt data</returns>
        [HttpPost("parse")]
        public ActionResult<CustomsReceipt> ParseCustomsReceipt([FromBody] ParseRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.OcrText))
                {
                    return BadRequest("OCR text is required");
                }

                _logger.LogInformation("Parsing customs receipt OCR text");

                var result = _parsingService.ParseOcrText(request.OcrText);

                // Output to console as requested
                var jsonOptions = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var jsonOutput = JsonSerializer.Serialize(result, jsonOptions);
                
                Console.WriteLine("=== Parsed Customs Receipt ===");
                Console.WriteLine(jsonOutput);
                Console.WriteLine("==============================");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input provided");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing customs receipt");
                return StatusCode(500, "An error occurred while parsing the customs receipt");
            }
        }

        /// <summary>
        /// Test the parser with sample Thai customs receipt data
        /// </summary>
        /// <returns>Parsed sample data</returns>
        [HttpGet("test")]
        public ActionResult<CustomsReceipt> TestParser()
        {
            try
            {
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

                _logger.LogInformation("Testing parser with sample data");

                var result = _parsingService.ParseOcrText(sampleOcrText);

                // Output to console as requested
                var jsonOptions = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var jsonOutput = JsonSerializer.Serialize(result, jsonOptions);
                
                Console.WriteLine("=== Test Parsing Result ===");
                Console.WriteLine(jsonOutput);
                Console.WriteLine("===========================");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test parser");
                return StatusCode(500, "An error occurred while testing the parser");
            }
        }
    }

    public class ParseRequest
    {
        public string OcrText { get; set; } = string.Empty;
    }
}