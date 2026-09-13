using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Model
{
    public record DocumentChunk(Guid DocumentId, string ChunkId, int StartIndex, string ChunkText);
}
