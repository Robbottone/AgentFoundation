using DotNetAIAgent.Model;
using System.Text;

namespace DotNetAIAgent.Generation;
public class RagMessageBuilder
{
    public string CreateRagMessage(IEnumerable<DocumentChunk> documentChunks, string query) 
    {
        var regText = new RagText() { Context = CreateContext(documentChunks), Query = query };

        return regText.ToString();
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