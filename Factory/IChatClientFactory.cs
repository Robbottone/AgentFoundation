using Microsoft.Extensions.AI;

namespace BasicHelloWorld.Factory
{
    public interface IChatClientFactory
    {
        IChatClient Create();
    }
}