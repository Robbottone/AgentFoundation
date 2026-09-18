using Microsoft.Extensions.AI;

namespace DotNetAIAgent.Interface
{
    public interface IEmbeddingClientFactory
    {
        IEmbeddingGenerator<string, Embedding<float>> Create();
    }
}
