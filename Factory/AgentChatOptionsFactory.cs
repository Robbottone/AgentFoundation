using DotNetAIAgent.Interface;
using DotNetAIAgent.Tools;
using Microsoft.Extensions.AI;

namespace DotNetAIAgent.Factory
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
