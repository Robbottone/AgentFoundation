namespace DotNetAIAgent.Model
{
    public record IndexedDocumentChunk(DocumentChunk documentChunk, ReadOnlyMemory<float> Vector);
}
