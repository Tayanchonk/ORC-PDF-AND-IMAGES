using Microsoft.AspNetCore.Mvc;
using OcrApi.Models;
using OcrApi.Services;

namespace OcrApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OcrController : ControllerBase
    {
        private readonly IOcrService _ocrService;
        private readonly ILogger<OcrController> _logger;

        public OcrController(IOcrService ocrService, ILogger<OcrController> logger)
        {
            _ocrService = ocrService;
            _logger = logger;
        }

        /// <summary>
        /// Upload a PDF or image file and extract text using OCR
        /// </summary>
        /// <param name="file">The file to process</param>
        /// <returns>OCR processing result with extracted text</returns>
        [HttpPost("upload-file")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(OcrResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<ActionResult<OcrResponse>> UploadFile([FromForm] IFormFile file)
        {
            try
            {
                _logger.LogInformation($"Received file upload request");

                var result = await _ocrService.ProcessImageAsync(file);

                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file upload");
                return StatusCode(500, $"An error occurred while processing the file: {ex.Message}");
            }
        }

        /// <summary>
        /// Health check endpoint to verify OCR service status
        /// </summary>
        /// <returns>Health status of the OCR service</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(HealthResponse), 200)]
        public async Task<ActionResult<HealthResponse>> HealthCheck()
        {
            try
            {
                var health = await _ocrService.GetHealthStatusAsync();
                return Ok(health);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking health status");
                return Ok(new HealthResponse
                {
                    Status = "unhealthy",
                    Error = ex.Message
                });
            }
        }
    }

    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// Root endpoint with API information
        /// </summary>
        /// <returns>API information and available endpoints</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiInfoResponse), 200)]
        public ActionResult<ApiInfoResponse> GetApiInfo()
        {
            return Ok(new ApiInfoResponse
            {
                Message = "OCR PDF and Images API (.NET Version)",
                Version = "1.0.0",
                Endpoints = new Dictionary<string, string>
                {
                    ["upload"] = "/api/ocr/upload-file",
                    ["health"] = "/api/ocr/health",
                    ["swagger"] = "/swagger"
                }
            });
        }
    }
}