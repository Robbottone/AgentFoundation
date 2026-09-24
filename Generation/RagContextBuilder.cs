using DotNetAIAgent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Generation
{
    public class RagContextBuilderService
    {
        public string CreateContext(IEnumerable<DocumentChunk> documentChunks)
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
}
