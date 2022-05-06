using Microsoft.Extensions.Logging;

namespace Plunger.APIs.WaifuPics;

internal class LoggerEvents
{
    internal EventId Startup = new(100, nameof(Startup));

    internal static EventId RestRx { get; } = new EventId(101, "REST ↓");
    internal static EventId RestTx { get; } = new EventId(102, "REST ↑");
}
