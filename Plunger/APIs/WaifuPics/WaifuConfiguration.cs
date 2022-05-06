using Microsoft.Extensions.Logging;

namespace Plunger.APIs.WaifuPics;

public class WaifuConfiguration
{
    public ILogger? Logger { internal get; set; }
    public string[]? DefaultExcludes { internal get; set; }
}
