using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
//using AITest.Helpers;

namespace AITest.Services;

public class ChatHistoryService
{
    private readonly Dictionary<string, ChatHistory> _chatHistories;

    public ChatHistoryService()
    {
        _chatHistories = [];
    }

    public ChatHistory GetOrCreateHistory(string sessionId)
    {
        if (!_chatHistories.TryGetValue(sessionId, out ChatHistory? history))
        {
            history = new ChatHistory("You are a friendly assistant that provides detailed descriptions of images");

            _chatHistories[sessionId] = history;
        }

        return history;
    }

    public void AddUserMessage(string textContent, string imageUri, string sessionId)
    {
        ChatMessageContentItemCollection message =
        [
            new TextContent(textContent),
            //new ImageContent(data: imageUri.ToReadOnlyMemory(),"image/png")
        ];

        _chatHistories[sessionId].AddUserMessage(message);
    }
}
