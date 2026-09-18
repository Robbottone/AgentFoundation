using DotNetAIAgent.Model;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetAIAgent.Embedding
{
    public class DocumentChunkingService
    {
        private IEnumerable<string> SplitOversizedParagraph(string paragraph, int chunkSize)
        { 
            var chunks = new List<string>();
            var buildingText = new StringBuilder();

            var sentences = Regex.Split(paragraph, @"\. (?=\w)");
            
            for (var i = 0; i < sentences.Length;) 
            {
                if (buildingText.Length == 0 && sentences[i].Length > chunkSize)
                {
                    //the idea is to take chunk of char[]
                    for(var offset = 0; offset < sentences[i].Length;) {
                        var remaining = sentences[i].Length - offset;
                        var length = Math.Min(chunkSize, remaining);

                        var subSentece = sentences[i].Substring(offset, length);

                        chunks.Add(subSentece);

                        offset += length;
                    }

                    i++;
                } 
                else 
                {
                    var separatorLength = buildingText.Length > 0 ? 2 : 0;

                    if ((buildingText.Length+separatorLength)+sentences[i].Length <= chunkSize)
                    {
                        if (buildingText.Length > 0)
                        {
                            buildingText.Append(". ");
                        }

                        buildingText.Append(sentences[i]);
                        i++;
                    }
                    else
                    {
                        chunks.Add(buildingText.ToString());
                        buildingText = new StringBuilder();
                    }
                }
            }

            if (buildingText.Length > 0)
            {
                chunks.Add(buildingText.ToString());
            }

            return chunks;
        }

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
            //each paragraph is divided by two newline \n
            var textParagraphed = normText.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
            var buildingText = new StringBuilder();

            for (var indexParagraph = 0; indexParagraph < textParagraphed.Length;)
            {
                //count the \n\n
                var separatorLength = buildingText.Length > 0 ? 2 : 0;

                //if current paragraf dimension + text gathered so far is < chunkSize -> append current paragraf
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
                    //if current paragraf > chunksize split paragraf in sub chuhks
                    if (buildingText.Length == 0 &&
                        textParagraphed[indexParagraph].Length > chunkSize)
                    {
                        var chunks = SplitOversizedParagraph(textParagraphed[indexParagraph], chunkSize);
                        
                        foreach(var chunk in chunks)
                        {
                            CreateDocumentChunk(document, documentChunk, normText, new StringBuilder(chunk));
                        }

                        indexParagraph++;
                    }
                    else
                    {
                        CreateDocumentChunk(document, documentChunk, normText, buildingText);
                        buildingText = new StringBuilder();
                    }
                }
            }

            if (buildingText.Length > 0)
            {
                CreateDocumentChunk(document, documentChunk, normText, buildingText);
            }

            return documentChunk;
        }

        private static void CreateDocumentChunk(KnowledgeDocument document, 
                                         List<DocumentChunk> documentChunk,
                                                           string normText,
                                                StringBuilder buildingText)
        {
            var lastDocumentChunk = documentChunk.LastOrDefault();

            var offset = (lastDocumentChunk?.StartIndex + lastDocumentChunk?.ChunkText.Length) ?? 0;
            int startingIndex = normText.IndexOf(buildingText.ToString(), offset);

            if (startingIndex == -1)
            {
                throw new InvalidOperationException("Non é possibile creare un documentChunk senza che la substring sia presente nel testo intero");
            }

            var chunkId = $"{document.DocumentId}_{startingIndex}";
            documentChunk.Add(new DocumentChunk(document.DocumentId, chunkId, startingIndex, buildingText.ToString()));
        }
    }
}
