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

            for (var indexParagraph = 0; indexParagraph < textParagraphed.Length;)
            {
                var separatorLength = buildingText.Length > 0 ? 2 : 0;

                if((buildingText.Length + separatorLength) + textParagraphed[indexParagraph].Length <= chunkSize)
                {
                    if (buildingText.Length > 0) 
                    {
                        buildingText.Append("\n\n");
                    }

                    buildingText.Append(textParagraphed[indexParagraph]);
                    indexParagraph++;
                }
                else
                {
                    if (textParagraphed[indexParagraph].Length > chunkSize)
                    {
                        
                    }

                    CreateDocumentChunk(document, documentChunk, normText, buildingText);

                    buildingText = new StringBuilder();
                }
            }

            if (buildingText.Length > 0)
            {
                CreateDocumentChunk(document, documentChunk, normText, buildingText);
            }

            return documentChunk;
        }

        private static void CreateDocumentChunk(KnowledgeDocument document, List<DocumentChunk> documentChunk, string normText, StringBuilder buildingText)
        {
            int startingIndex = normText.LastIndexOf(buildingText.ToString());
            var chunkId = $"{document.DocumentId}_{startingIndex}";
            documentChunk.Add(new DocumentChunk(document.DocumentId,
                                                            chunkId,
                                                      startingIndex,
                                           buildingText.ToString()));
        }
    }
}
