using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Model
{
    public record IndexedDocumentChunk(DocumentChunk documentChunk, ReadOnlyMemory<float> Vector);
}
