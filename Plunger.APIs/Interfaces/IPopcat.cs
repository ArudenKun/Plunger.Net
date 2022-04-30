using Plunger.APIs.Models;
using Refit;

namespace Plunger.APIs.Interfaces;

public interface IPopcat
{
    [Get("/encode?text={text}")]
    Task<PopcatResponse> BinaryEncode(string text);
    [Get("/decode?binary={binary}")]
    Task<PopcatResponse> BinaryDecode(string binary);
}
