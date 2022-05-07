using Plunger.APIs.Reddit.Responses;
using Refit;

namespace Plunger.APIs.Reddit;

public interface IRedditClient
{
    [Get("/r/creampies/random.json")]
    Task<string> Creampie();
}
