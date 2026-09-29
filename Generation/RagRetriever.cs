using DotNetAIAgent.Embedding;
using DotNetAIAgent.Model;
using Microsoft.Extensions.AI;
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

        public RagRetriever(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, VectorSearchService vectorSearch)
        {
            _embeddingGenerator = embeddingGenerator;
            _vectorSearch = vectorSearch;
        }

        public async Task<IEnumerable<DocumentChunk>> RetrieveAsync(string query, IEnumerable<IndexedDocumentChunk> indexedChunks, int topK = 3)
        { 
            var queryEmbedding = await _embeddingGenerator.GenerateAsync([query]);
            var vectorQuery = queryEmbedding.First();

            var indexedTextResult = _vectorSearch.Search(vectorQuery.Vector, indexedChunks, topK);
            
            return indexedTextResult.Select(el => el.IndexedDocument.DocumentChunk);
        }
    }
}
