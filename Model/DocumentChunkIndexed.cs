using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Model
{
    public record DocumentChunkIndexed(DocumentChunk documentChunk, ReadOnlyMemory<float> Vector);
}
