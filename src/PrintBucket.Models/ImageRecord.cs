using System;
using System.Text.Json.Serialization;

namespace PrintBucket.Models
{
    public class ImageRecord
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("bucketId")]
        public string BucketId { get; set; } = string.Empty;

        [JsonPropertyName("originalKey")]
        public string OriginalKey { get; set; } = string.Empty;

        [JsonPropertyName("largeKey")]
        public string LargeKey { get; set; } = string.Empty;

        [JsonPropertyName("smallKey")]
        public string SmallKey { get; set; } = string.Empty;

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}