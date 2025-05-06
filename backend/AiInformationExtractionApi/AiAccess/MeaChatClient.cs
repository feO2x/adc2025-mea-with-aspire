using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;

namespace AiInformationExtractionApi.AiAccess;

public sealed class MeaChatClient : IAiChatClient
{
    private readonly IChatClient _chatClient;

    public MeaChatClient(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public Task<ChatResponse> CompleteChatAsync(
        List<ChatMessage> chatMessages,
        CancellationToken cancellationToken = default
    )
    {
        var options = new ChatOptions
        {
            Temperature = 0.0f,
            ResponseFormat = ChatResponseFormat.Json
        };
        return _chatClient.GetResponseAsync(chatMessages, options, cancellationToken);
    }
}
