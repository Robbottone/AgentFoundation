using Microsoft.Extensions.AI;

namespace DotNetAIAgent.Interface
{
    public interface IChatClientFactory
    {
        IChatClient Create();
    }
}