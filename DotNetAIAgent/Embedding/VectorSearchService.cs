using DotNetAIAgent.Model;

namespace DotNetAIAgent.Embedding
{
    public class VectorSearchService
    {
        public IEnumerable<SearchResultEmbedding> Search(ReadOnlyMemory<float> vectorQuery, IEnumerable<IndexedDocumentChunk> indexedDocumentChunks, int topK = 3, float? similarityThreshold = null)
        {
            if (vectorQuery.IsEmpty)
               throw new ArgumentException("Il vettore della query non deve essere vuoto");

            if (!indexedDocumentChunks.Any())
               throw new ArgumentException("I vettori della ricerca devono essere presenti");

            var searchResultCosineValues = new List<SearchResultEmbedding>();

            foreach (var indexedText in indexedDocumentChunks)
            {
                var cosineValue = CosineSimilarity(vectorQuery, indexedText.Vector);

                searchResultCosineValues.Add(new(indexedText, cosineValue));
            }

            if (similarityThreshold.HasValue)
            { 
                return searchResultCosineValues.Where(el => el.Similarity >= similarityThreshold.Value).OrderByDescending(el => el.Similarity).Take(topK);
            }

            return searchResultCosineValues.OrderByDescending(el => el.Similarity).Take(topK);
        }

        private float CosineSimilarity(ReadOnlyMemory<float> vectorA, ReadOnlyMemory<float> vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new ArgumentException("I vettori devono avere la stessa dimensione.");

            float dotProduct = 0f;
            float squareSumA = 0f;
            float squareSumB = 0f;

            ReadOnlySpan<float> spanA = vectorA.Span;
            ReadOnlySpan<float> spanB = vectorB.Span;

            for (int i = 0; i < spanA.Length; i++)
            {
                float a = spanA[i];
                float b = spanB[i];

                dotProduct += a * b;
                squareSumA += a * a;
                squareSumB += b * b;
            }

            float magnitudeA = MathF.Sqrt(squareSumA);
            float magnitudeB = MathF.Sqrt(squareSumB);

            if (magnitudeA == 0f || magnitudeB == 0f)
                return 0f;

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
