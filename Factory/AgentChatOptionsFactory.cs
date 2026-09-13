using BasicHelloWorld.Tools;
using DotNetAIAgent.Interface;
using Microsoft.Extensions.AI;

namespace BasicHelloWorld.Factory
{
    public class AgentChatOptionsFactory : IChatOptionsFactory
    {
        private readonly AIToolRegistry _toolRegistry;

        public AgentChatOptionsFactory(AIToolRegistry toolRegistry)
        {
            _toolRegistry = toolRegistry;
        }

        public ChatOptions Create()
        {
            var chatOptions = new ChatOptions
            {
                Tools = _toolRegistry.GetTools()
            };

            return chatOptions;
        }
    }
}
