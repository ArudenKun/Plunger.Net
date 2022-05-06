using Plunger.APIs.WaifuPics.Entities;
using Plunger.Commons.Extensions;

namespace Plunger.APIs.WaifuPics;

public class Endpoints
{
    private const string BaseUrl = "https://api.waifu.pics";

    internal static string GetSfwEndpoint(SfwCategory category, bool getMany = false)
    {
        return getMany switch
        {
            true => $"{BaseUrl}/many/sfw/{category.GetValue()}",
            false => $"{BaseUrl}/sfw/{category.GetValue()}"
        };
    }

    internal static string GetNsfwEndpoint(NsfwCategory category, bool getMany = false)
    {
        return getMany switch
        {
            true => $"{BaseUrl}/many/nsfw/{category.GetValue()}",
            false => $"{BaseUrl}/nsfw/{category.GetValue()}"
        };
    }
}
