using Plunger.APIs.Models;
using Plunger.APIs.Models.Enums;
using Refit;

namespace Plunger.APIs.Interfaces;

public interface IWaifu
{
    [Get("/sfw/{category}")]
    Task<WaifuPicsImage> GetSfwImageAsync(SfwCategory category);

    [Get("/nsfw/{category}")]
    Task<WaifuPicsImage> GetNsfwImageAsync(NsfwCategory category);
}