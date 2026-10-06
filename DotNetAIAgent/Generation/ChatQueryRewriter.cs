using Microsoft.Extensions.AI;
using System.Text;

namespace DotNetAIAgent.Generation;

public class ChatQueryRewriter
{
    private readonly IChatClient _chatClient;

    public ChatQueryRewriter(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> RewriteQueryAsync(string query, IEnumerable<ChatMessage> history)
    {
        ArgumentException.ThrowIfNullOrEmpty(query);
        ArgumentNullException.ThrowIfNull(history);

        var stringBuilder = new StringBuilder();
        var historyInfo = history.Append(new ChatMessage(ChatRole.User, query));

        var chatResponse = _chatClient.GetStreamingResponseAsync(historyInfo, new ChatOptions() { Instructions = "Riscrivi la nuova domanda usando la cronologia per renderla autonoma e comprensibile senza contesto precedente. Non rispondere alla domanda."});

        await foreach (var item in chatResponse)
        {
            if (!string.IsNullOrEmpty(item.Text))
                stringBuilder.Append(item.Text);
        }

        return stringBuilder.ToString();
    }
}
