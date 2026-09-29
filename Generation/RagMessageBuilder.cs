using DotNetAIAgent.Model;
using System.Text;

namespace DotNetAIAgent.Generation;
public class RagMessageBuilder
{
    public string CreateRagMessage(IEnumerable<DocumentChunk> documentChunks, string query)
    { 
        var context = CreateContext(documentChunks);

        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendLine($"CONTEXT:\n{context}");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine($"QUERY:\n{query}");
        stringBuilder.AppendLine();

        return stringBuilder.ToString();
    }

    private string CreateContext(IEnumerable<DocumentChunk> documentChunks)
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

        return stringBuilder.ToString();
    }
}