using DotNetAIAgent.Model;
using Microsoft.Extensions.AI;

namespace DotNetAIAgent.Embedding
{
    public class DocumentChunkIndexService
    {
        IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

        public DocumentChunkIndexService(IEmbeddingGenerator<string, Embedding<float>> embedding)
        {
            _embeddingGenerator = embedding;
        }

        public async IAsyncEnumerable<DocumentChunkIndexed> GenerateDocumentChunkIndex(IEnumerable<DocumentChunk> documentChunks)
        {
            if (documentChunks is null)
                throw new ArgumentNullException(nameof(documentChunks), "La collezione dei document chunks non puo essere pari a null");

           // if (!documentChunks.Any()) //non c'e' passaggio di elementi e fine viene concluso senza nessun elemento restituito.
            
            foreach(var documentChunk in documentChunks) {
                var vector = await _embeddingGenerator.GenerateVectorAsync(documentChunk.ChunkText);
                var documentChunkIndex = new DocumentChunkIndexed(documentChunk, vector);

                yield return documentChunkIndex;
            }
        }
    }
}
