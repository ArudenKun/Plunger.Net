using System.Text.Json.Serialization;

namespace Plunger.APIs.Waifu.Im.Responses;

public class WaifuImResponse
{
    [JsonPropertyName("images")]
    public List<Image>? Images { get; set; }

    public class Image
    {
        [JsonPropertyName("file")]
        public string? File { get; set; }

        [JsonPropertyName("extension")]
        public string? Extension { get; set; }

        [JsonPropertyName("image_id")]
        public long ImageId { get; set; }

        [JsonPropertyName("favourites")]
        public long Favourites { get; set; }

        [JsonPropertyName("dominant_color")]
        public string? DominantColor { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("uploaded_at")]
        public DateTimeOffset UploadedAt { get; set; }

        [JsonPropertyName("is_nsfw")]
        public bool IsNsfw { get; set; }

        // [JsonPropertyName("width")]
        // [JsonConverter(typeof(ParseStringConverter))]
        // public int Width { get; set; }

        // [JsonPropertyName("height")]
        // [JsonConverter(typeof(ParseStringConverter))]
        // public int Height { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("preview_url")]
        public string? PreviewUrl { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag>? Tags { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("tag_id")]
        public long TagId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("is_nsfw")]
        public bool IsNsfw { get; set; }
    }
}