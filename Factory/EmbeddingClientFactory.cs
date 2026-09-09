using Azure.AI.OpenAI;
using Azure.Identity;
using DotNetAIAgent.Interface;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace BasicHelloWorld.Factory
{
    public class EmbeddingClientFactory: IEmbeddingClientFactory
    {
        private readonly AgentConnectionOptions _agentConnectionOption;

        public EmbeddingClientFactory(IOptions<AgentConnectionOptions> agentOptions) {
            _agentConnectionOption = agentOptions.Value;
        }

        // costruzione del client
        public IEmbeddingGenerator<string, Embedding<float>> Create()
        {
            
            var endpoint = new Uri(_agentConnectionOption.Endpoint);

            var azureClient = new AzureOpenAIClient(
                endpoint,
                new AzureCliCredential());

            var embeddingClient = azureClient.GetEmbeddingClient(_agentConnectionOption.EmbeddingDeployment).AsIEmbeddingGenerator();
            
            return embeddingClient;
        }
    }
}
