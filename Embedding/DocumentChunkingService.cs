using DotNetAIAgent.Model;

namespace DotNetAIAgent.Embedding
{
    public class DocumentChunkingService
    {
        public IEnumerable<DocumentChunk> GenerateDocumentChunk(KnowledgeDocument document, int chunkSize, int chunkOverlap)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (string.IsNullOrEmpty(document.Text))
            {
                return new List<DocumentChunk>();
            }

            if (chunkSize <= 0)
            { 
                throw new ArgumentOutOfRangeException(nameof(chunkSize), "La dimensione del chunk deve essere maggiore di zero.");
            }

            if (chunkOverlap >= chunkSize)
            { 
                throw new ArgumentOutOfRangeException(nameof(chunkOverlap), "La dimensione del chunk overlap deve essere minore del chunkSize.");
            }

            var documentChunk = new List<DocumentChunk>();

            var step = chunkSize - chunkOverlap;

            for (var i = 0; i < document.Text.Length; i+=step)
            {
                var chunkId = $"{document.DocumentId}_{i}";
                var text = new string(document.Text.Skip(i).Take(chunkSize).ToArray());

                documentChunk.Add(new DocumentChunk(document.DocumentId, chunkId, i, text));

                if (i+chunkSize >= document.Text.Length)
                    break;
            }

            return documentChunk;
        }
    }
}
