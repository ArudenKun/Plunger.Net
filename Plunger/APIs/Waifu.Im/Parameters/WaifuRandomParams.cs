using Refit;

namespace Plunger.APIs.Waifu.Im.Parameters;

public class WaifuRandomParams<T>
{
    [AliasAs("is_nsfw")]
    public bool? IsNsfw { get; set; } = null;

    [AliasAs("selected_tags")]
    public T? SelectedTags { get; set; }

    [AliasAs("gif")]
    public bool Gif {get; set;}
}