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
        private readonly ChatQueryRewriter _chatQueryRewriter;

        public RagRetriever(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, VectorSearchService vectorSearch, ChatQueryRewriter chatQueryRewriter)
        {
            _embeddingGenerator = embeddingGenerator;
            _vectorSearch = vectorSearch;
            _chatQueryRewriter = chatQueryRewriter;
        }

        public async Task<IEnumerable<DocumentChunk>> RetrieveAsync(IEnumerable<IndexedDocumentChunk> indexedChunks, string query, IEnumerable<ChatMessage> messages, int topK = 3)
        { 
            var rewroteQuery = await _chatQueryRewriter.RewriteQueryAsync(query, messages);

            var queryEmbedding = await _embeddingGenerator.GenerateAsync([rewroteQuery]);
            var vectorQuery = queryEmbedding.First();

            var indexedTextResult = _vectorSearch.Search(vectorQuery.Vector, indexedChunks, topK);
            
            return indexedTextResult.Select(el => el.IndexedDocument.DocumentChunk);
        }
    }
}
