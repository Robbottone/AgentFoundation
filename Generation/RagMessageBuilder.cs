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
    public class RagMessageBuilder
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly VectorSearchService _vectorSearch;

        public RagMessageBuilder(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, VectorSearchService vectorSearch)
        {
            _embeddingGenerator = embeddingGenerator;
            _vectorSearch = vectorSearch;
        }

        public async Task<string> RetrieveMessageAsync(string query, IEnumerable<IndexedDocumentChunk> indexedChunks, int topK = 3)
        { 
            var queryEmbedding = await _embeddingGenerator.GenerateAsync([query]);
            var vectorQuery = queryEmbedding.First();

            var indexedTextResult = _vectorSearch.Search(vectorQuery.Vector, indexedChunks, topK);
            
            return CreateMessage(indexedTextResult.Select(el => el.IndexedDocument.DocumentChunk), query);
        }

        private static string CreateMessage(IEnumerable<DocumentChunk> documentChunks, string query)
        { 
            var stringBuilder = new StringBuilder();
            var id = 1;

            foreach (var chunk in documentChunks)
            {
                stringBuilder.AppendLine($"----- Source {id} -----");
                stringBuilder.AppendLine(chunk.ChunkText);
                stringBuilder.AppendLine();
                id++;
            }

            stringBuilder.AppendLine(query);
            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    }
}
