namespace DotNetAIAgent.Model
{
    public record IndexedText(string Text, ReadOnlyMemory<float> Vector);
    public record SearchResultEmbedding(IndexedText IndexedText, float Similarity);
}
