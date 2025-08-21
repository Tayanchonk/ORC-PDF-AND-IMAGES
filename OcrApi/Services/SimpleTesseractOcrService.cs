using OcrApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Text.Json;

namespace OcrApi.Services
{
    public class SimpleTesseractOcrService : IOcrService
    {
        private readonly ILogger<SimpleTesseractOcrService> _logger;
        
        // Supported image types
        private readonly HashSet<string> _supportedImageTypes = new HashSet<string>
        {
            "image/jpeg",
            "image/jpg", 
            "image/png",
            "image/tiff",
            "image/tif",
            "image/bmp",
            "image/webp"
        };

        private readonly HashSet<string> _supportedPdfTypes = new HashSet<string>
        {
            "application/pdf"
        };

        public SimpleTesseractOcrService(ILogger<SimpleTesseractOcrService> logger)
        {
            _logger = logger;
        }

        public async Task<OcrResponse> ProcessImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new OcrResponse
                    {
                        Success = false,
                        Message = "No file provided or file is empty"
                    };
                }

                _logger.LogInformation($"Processing file: {file.FileName}, Content-Type: {file.ContentType}, Size: {file.Length} bytes");

                // Validate file type
                if (!IsValidImageType(file.ContentType) && !IsValidPdfType(file.ContentType))
                {
                    return new OcrResponse
                    {
                        Success = false,
                        Message = $"Unsupported file type: {file.ContentType}. Supported types: {string.Join(", ", _supportedImageTypes.Union(_supportedPdfTypes))}"
                    };
                }

                // Read file content
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                string extractedText;
                OcrData data;

                if (IsValidPdfType(file.ContentType))
                {
                    // For now, return a message that PDF processing is not yet implemented
                    return new OcrResponse
                    {
                        Success = false,
                        Message = "PDF processing is not yet implemented in this version. Please convert PDF to images first."
                    };
                }
                else
                {
                    // Process as image
                    extractedText = await ExtractTextFromImageAsync(fileContent);
                    
                    data = new OcrData
                    {
                        Filename = file.FileName ?? "unknown",
                        FileType = file.ContentType,
                        ExtractedText = extractedText,
                        CharacterCount = extractedText?.Length ?? 0
                    };
                }

                // Log to console as requested in original requirements
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var response = new OcrResponse
                {
                    Success = true,
                    Message = "File processed successfully",
                    Data = data
                };

                var jsonOutput = JsonSerializer.Serialize(response, jsonOptions);
                Console.WriteLine("=== OCR Processing Result ===");
                Console.WriteLine(jsonOutput);
                Console.WriteLine("=============================");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file");
                return new OcrResponse
                {
                    Success = false,
                    Message = $"Error processing file: {ex.Message}"
                };
            }
        }

        public async Task<string> ExtractTextFromImageAsync(byte[] imageData)
        {
            try
            {
                // Create a temporary file for the image
                var tempImagePath = Path.GetTempFileName() + ".png";
                var tempOutputPath = Path.GetTempFileName();

                try
                {
                    // Save image data to temporary file
                    using (var image = Image.Load<Rgba32>(imageData))
                    {
                        await image.SaveAsPngAsync(tempImagePath);
                    }

                    // Call tesseract binary directly
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "tesseract",
                        Arguments = $"\"{tempImagePath}\" \"{tempOutputPath}\" -l eng",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    if (process == null)
                    {
                        throw new InvalidOperationException("Failed to start tesseract process");
                    }

                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        var error = await process.StandardError.ReadToEndAsync();
                        throw new InvalidOperationException($"Tesseract failed with exit code {process.ExitCode}: {error}");
                    }

                    // Read the output file
                    var outputFile = tempOutputPath + ".txt";
                    if (File.Exists(outputFile))
                    {
                        var text = await File.ReadAllTextAsync(outputFile);
                        _logger.LogInformation($"OCR completed. Text length: {text.Length}");
                        return text.Trim();
                    }
                    else
                    {
                        _logger.LogWarning("Tesseract output file not found");
                        return string.Empty;
                    }
                }
                finally
                {
                    // Clean up temporary files
                    try
                    {
                        if (File.Exists(tempImagePath)) File.Delete(tempImagePath);
                        if (File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
                        if (File.Exists(tempOutputPath + ".txt")) File.Delete(tempOutputPath + ".txt");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clean up temporary files");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from image");
                throw new InvalidOperationException($"OCR processing failed: {ex.Message}", ex);
            }
        }

        public async Task<HealthResponse> GetHealthStatusAsync()
        {
            try
            {
                // Test if tesseract binary is available
                var startInfo = new ProcessStartInfo
                {
                    FileName = "tesseract",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    return new HealthResponse
                    {
                        Status = "unhealthy",
                        Error = "Could not start tesseract process"
                    };
                }

                await process.WaitForExitAsync();
                
                if (process.ExitCode == 0)
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var firstLine = output.Split('\n')[0];
                    
                    return new HealthResponse
                    {
                        Status = "healthy",
                        OcrEngine = firstLine.Trim()
                    };
                }
                else
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    return new HealthResponse
                    {
                        Status = "unhealthy",
                        Error = $"Tesseract exit code {process.ExitCode}: {error}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return new HealthResponse
                {
                    Status = "unhealthy",
                    Error = ex.Message
                };
            }
        }

        public bool IsValidImageType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && _supportedImageTypes.Contains(contentType.ToLowerInvariant());
        }

        public bool IsValidPdfType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && _supportedPdfTypes.Contains(contentType.ToLowerInvariant());
        }
    }
}