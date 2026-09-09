using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Model
{
    public record IndexedText(string Text, ReadOnlyMemory<float> Vector);
    public record SearchResultEmbedding(IndexedText IndexedText, float Similarity);
}
