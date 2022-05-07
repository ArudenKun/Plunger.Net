using Refit;

namespace Plunger.APIs.Hastebin;

public interface IHastebinClient
{
    [Post("/documents")]
    Task<string> UploadAsync([Body]string code); 
}
