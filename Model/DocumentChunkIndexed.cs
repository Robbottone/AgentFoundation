namespace DotNetAIAgent.Model
{
    public record DocumentChunkIndexed(DocumentChunk documentChunk, ReadOnlyMemory<float> Vector);
}
