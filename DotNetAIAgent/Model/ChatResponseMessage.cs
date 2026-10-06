using Microsoft.Extensions.AI;
using static System.Net.Mime.MediaTypeNames;

namespace DotNetAIAgent.Model;
public record ChatResponseMessage(string Text, int InputTokens, int OutputTokens, ChatFinishReason? FinishReason);
