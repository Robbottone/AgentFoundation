using Microsoft.Extensions.AI;

namespace BasicHelloWorld.Factory
{
    public interface IChatOptionsFactory
    {
        ChatOptions Create();
    }
}
