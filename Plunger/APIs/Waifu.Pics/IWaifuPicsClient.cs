using Plunger.APIs.Waifu.Pics.Enums;
using Refit;

namespace Plunger.APIs.Waifu.Pics;

public interface IWaifuPicsClient
{
    [Get("/sfw/{category}")]
    Task<string> GetSfwImageAsync(SfwCategory category);

    [Get("/nsfw/{category}")]
    Task<string> GetNsfwImageAsync(NsfwCategory category);
}
