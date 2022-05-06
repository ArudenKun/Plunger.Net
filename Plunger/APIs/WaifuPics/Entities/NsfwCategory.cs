using Plunger.Commons.Attributes;

namespace Plunger.APIs.WaifuPics.Entities;

public enum NsfwCategory
{
    [Value("waifu")]
    Waifu,

    [Value("neko")]
    Neko,

    [Value("trap")]
    Trap,

    [Value("blowjob")]
    Blowjob
}
