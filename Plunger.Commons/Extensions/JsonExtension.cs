using System.Text.Json;

namespace Plunger.Commons.Extensions;

public static class JsonExtension
{
    public static async Task<T> DeserializeAsync<T>(this HttpResponseMessage response, JsonSerializerOptions options, CancellationToken token)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(token);
#pragma warning disable CS8603 // Possible null reference return.
        return await JsonSerializer.DeserializeAsync<T>(stream, options, token);
#pragma warning restore CS8603 // Possible null reference return.
    }
}
