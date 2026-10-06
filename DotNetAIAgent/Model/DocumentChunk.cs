namespace DotNetAIAgent.Model
{
    public record DocumentChunk(Guid DocumentId, string ChunkId, int StartIndex, string ChunkText);
}
