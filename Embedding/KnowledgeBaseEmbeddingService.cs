using DotNetAIAgent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Embedding
{
    public class KnowledgeBaseEmbeddingService
    {
        private readonly DocumentChunkingService _documentChunkingService;
        private readonly DocumentChunkIndexService _documentChunkIndexService;
        
        public KnowledgeBaseEmbeddingService(DocumentChunkingService documentChunkingService, DocumentChunkIndexService documentChunkIndexService)
        {
            _documentChunkingService = documentChunkingService;
            _documentChunkIndexService = documentChunkIndexService;
        }

        public async Task<IEnumerable<IndexedDocumentChunk>> CreateEmbeddings(KnowledgeDocument knowledge)
        { 
            var documentChunks = _documentChunkingService.GenerateDocumentChunk(knowledge, 400, 80);

            var indexedDocumentChunks = _documentChunkIndexService.GenerateDocumentChunkIndex(documentChunks);

            List<IndexedDocumentChunk> chunks = new();

            await foreach (var indChunk in indexedDocumentChunks)
            {
                chunks.Add(indChunk);
            } 

            return chunks;
        }
    }
}
