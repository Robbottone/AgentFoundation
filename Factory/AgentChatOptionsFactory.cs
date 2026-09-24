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
                Tools = _toolRegistry.GetTools(),
                Instructions = "Rispondi alla domanda utilizzando esclusivamente\r\nle informazioni presenti nel CONTEXT fornito.\r\n\r\nSe il CONTEXT non contiene informazioni sufficienti\r\nper rispondere, dichiaralo senza inventare informazioni.\r\n\r\nIndica le SOURCE utilizzate per formulare la risposta."
            };

            return chatOptions;
        }
    }
}
