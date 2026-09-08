using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicHelloWorld
{
    public class AgentConnectionOptions
    {
        public string Endpoint   { get; set; } = string.Empty;
        public string Deployment { get; set; } = string.Empty;
    }
}
