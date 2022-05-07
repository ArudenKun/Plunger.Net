using Plunger.APIs.Popcat.Paremeters;
using Plunger.APIs.Popcat.Responses;
using Refit;

namespace Plunger.APIs.Popcat;

public interface IPopcatClient
{
    [Get("/encode")]
    Task<BinaryResponse> Encode([AliasAs("text")]string input);

    [Get("/decode")]
    Task<BinaryResponse> Decode([AliasAs("binary")]string input);

    [Get("/chatbot")]
    Task<ChatbotResponse> Chatbot(ChatbotParams parameters);
}