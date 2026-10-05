using DotNetAIAgent.Model;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.Text;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace DotNetAIAgent.Generation;

public class ChatResponseService
{
    private readonly IChatClient _chatClient;

    public ChatResponseService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<ChatResponseMessage> GenerateResponseAsync(List<ChatMessage> messages, ChatOptions options)
    { 
        var sb = new StringBuilder();
        var inputToken = 0;
        var outputToken = 0;
        ChatFinishReason? finishReason = null;
    
        var responses = _chatClient.GetStreamingResponseAsync(messages, options);

        await foreach (var item in responses)
        {
            if(!string.IsNullOrEmpty(item.Text))
            {
                sb.Append(item.Text);
            }

            if (item.RawRepresentation is StreamingChatCompletionUpdate metaDataRawChatUpdate)
            {
                if (metaDataRawChatUpdate?.Usage is not null)
                {
                    inputToken += metaDataRawChatUpdate.Usage.InputTokenCount;
                    outputToken += metaDataRawChatUpdate.Usage.OutputTokenCount;
                }
            }

            if (item.FinishReason is not null)
            {
                finishReason = item.FinishReason;
            }

        }

        return new ChatResponseMessage(sb.ToString(), inputToken, outputToken, finishReason);
    }
}
