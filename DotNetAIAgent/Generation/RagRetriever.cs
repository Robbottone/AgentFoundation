using DotNetAIAgent.Embedding;
using DotNetAIAgent.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Generation
{
    public class RagRetriever
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly VectorSearchService _vectorSearch;
        private readonly RetrievalOptions _retrievalOptions;

        public RagRetriever(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, VectorSearchService vectorSearch, IOptions<RetrievalOptions> options)
        {
            _embeddingGenerator = embeddingGenerator;
            _vectorSearch = vectorSearch;
            _retrievalOptions = options.Value;
        }

        private async Task<IEnumerable<SearchResultEmbedding>> RetrieveDocumentWithSimilarityAsync(IEnumerable<IndexedDocumentChunk> indexedChunks, string query, int topK)
        {
            var queryEmbedding = await _embeddingGenerator.GenerateAsync([query]);
            var vectorQuery = queryEmbedding.First();

            var indexedTextResult = _vectorSearch.Search(vectorQuery.Vector, indexedChunks, topK, _retrievalOptions.SimilarityThreshold);
            return indexedTextResult;
        }

        public async Task<IEnumerable<DocumentChunk>> RetrieveAsync(IEnumerable<IndexedDocumentChunk> indexedChunks, string query, int topK = 3)
        {
            IEnumerable<SearchResultEmbedding> indexedTextResult = await RetrieveDocumentWithSimilarityAsync(indexedChunks, query, topK);

            return indexedTextResult.Select(el => el.IndexedDocument.DocumentChunk);
        }

        public async Task<IEnumerable<SearchResultEmbedding>> RetrieveWithScoresAsync(IEnumerable<IndexedDocumentChunk> indexedChunks, string query, int topK = 3)
        { 
            return await this.RetrieveDocumentWithSimilarityAsync(indexedChunks, query, topK);
        }

        
    }
}
