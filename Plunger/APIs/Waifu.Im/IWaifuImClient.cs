using Plunger.APIs.Waifu.Im.Endpoints;
using Plunger.APIs.Waifu.Im.Parameters;
using Refit;

namespace Plunger.APIs.Waifu.Im;

public interface IWaifuImClient
{
    [Get("/random/")]
    Task<string> GetRandomNsfwAsync(WaifuRandomParams<WaifuImNsfwTags> parameters);

    [Get("/random/")]
    Task<string> GetRandomSfwAsync(WaifuRandomParams<WaifuImVersatileTags> parameters);
}