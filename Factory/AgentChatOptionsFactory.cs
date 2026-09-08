using BasicHelloWorld.Tools;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
