using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetAIAgent.Model
{
    public class RagText
    {
        public string Query { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"CONTEXT:\n{this.Context}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"QUERY:\n{this.Query}");
            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    }
}
