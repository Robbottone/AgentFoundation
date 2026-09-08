using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicHelloWorld.Factory
{
    public class ChatClientFactory: IChatClientFactory
    {
        private readonly AgentConnectionOptions _agentConnectionOption;

        public ChatClientFactory(IOptions<AgentConnectionOptions> agentOptions) {
            _agentConnectionOption = agentOptions.Value;
        }

        // costruzione del client
        public IChatClient Create()
        {
            
            var endpoint = new Uri(_agentConnectionOption.Endpoint);

            var azureClient = new AzureOpenAIClient(
                endpoint,
                new AzureCliCredential());

            var chatClient = azureClient.GetChatClient(_agentConnectionOption.Deployment).AsIChatClient();

            var builder = new ChatClientBuilder(chatClient);
            builder.UseFunctionInvocation();

            return builder.Build();
        }
    }
}
