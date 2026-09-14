using DotNetAIAgent.Model;
using System.Text;

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

            var normText = document.Text.Replace("\r\n","\n");
            var textParagraphed = normText.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
            var buildingText = new StringBuilder();

            var startingIndex = 0;

            for (var indexParagraph = 0; indexParagraph < textParagraphed.Length;)
            {
                if(buildingText.Length + textParagraphed[indexParagraph].Length < chunkSize)
                {
                    buildingText.Append(textParagraphed[indexParagraph]);
                    indexParagraph++;
                }
                else
                {
                    startingIndex = buildingText.Length;
                    var chunkId = $"{document.DocumentId}_{startingIndex}";
                    documentChunk.Add(new DocumentChunk(document.DocumentId, 
                                                                    chunkId,
                                                   indexParagraph*chunkSize,
                                                   buildingText.ToString()));

                    buildingText = new StringBuilder();
                }
            }

            return documentChunk;
        }
    }
}
