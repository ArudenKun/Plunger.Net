using System.Text;
using Newtonsoft.Json;

namespace Plunger.APIs;

public class Hastebin
{
    public const string HastebinEndpoint = "https://www.toptal.com/developers/hastebin";

    public const string HatebinEndpoint = "https://hatebin.com";

    private readonly IHttpClientFactory _httpClientFactory;

    public Hastebin(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> UploadAsync(string content)
    {
        using var stringContent = new StringContent(content, Encoding.UTF8);
        var _client = _httpClientFactory.CreateClient();
        var response = await _client.PostAsync(new Uri($"{HastebinEndpoint}/documents"), stringContent);

        if (!response.IsSuccessStatusCode)
        {
            // Fallback to Hatebin

            using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "text", content }
                });

            response = await _client.PostAsync(new Uri($"{HatebinEndpoint}/index.php"), formContent);
            response.EnsureSuccessStatusCode();

            string key = await response.Content.ReadAsStringAsync();
            return GetUrl(HatebinEndpoint, key);
        }

        string json = await response.Content.ReadAsStringAsync();
        var temp = JsonConvert.DeserializeObject<HastebinResponse>(json);
        return GetUrl(HastebinEndpoint, temp!.Key!);
    }

    private static string GetUrl(string endpoint, string key)
    {
        return $"{endpoint}/{key.Trim()}";
    }

    private class HastebinResponse
    {
        [JsonProperty]
        public string? Key { get; private set; }
    }
}
