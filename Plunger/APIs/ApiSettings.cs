using System.Text.Json;
using Refit;

namespace Plunger.APIs;

public static class ApiSettings
{
    public static readonly RefitSettings Settings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        })
    };
}
