using System.Text.Json.Serialization;

namespace OcrApi.Models
{
    public class FileUploadRequest
    {
        public IFormFile? File { get; set; }
    }

    public class OcrResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public OcrData Data { get; set; } = new OcrData();
    }

    public class OcrData
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; } = string.Empty;

        [JsonPropertyName("file_type")]
        public string FileType { get; set; } = string.Empty;

        [JsonPropertyName("extracted_text")]
        public string? ExtractedText { get; set; }

        [JsonPropertyName("character_count")]
        public int? CharacterCount { get; set; }

        [JsonPropertyName("total_pages")]
        public int? TotalPages { get; set; }

        [JsonPropertyName("pages")]
        public List<PageData>? Pages { get; set; }

        [JsonPropertyName("full_text")]
        public string? FullText { get; set; }

        [JsonPropertyName("total_characters")]
        public int? TotalCharacters { get; set; }
    }

    public class PageData
    {
        [JsonPropertyName("page_number")]
        public int PageNumber { get; set; }

        [JsonPropertyName("extracted_text")]
        public string ExtractedText { get; set; } = string.Empty;

        [JsonPropertyName("character_count")]
        public int CharacterCount { get; set; }
    }

    public class HealthResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("ocr_engine")]
        public string? OcrEngine { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    public class ApiInfoResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("endpoints")]
        public Dictionary<string, string> Endpoints { get; set; } = new Dictionary<string, string>();
    }
}