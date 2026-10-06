using Microsoft.Extensions.AI;

namespace DotNetAIAgent.Interface
{
    public interface IChatOptionsFactory
    {
        ChatOptions Create();
    }
}
