namespace DotNetAIAgent.Model
{
    public record IndexedDocumentChunk(DocumentChunk DocumentChunk, ReadOnlyMemory<float> Vector);
}
