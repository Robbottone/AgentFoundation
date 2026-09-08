using BasicHelloWorld.Attribute;
using BasicHelloWorld.Model.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicHelloWorld.Tools
{
    public class AIToolRegistry
    {
        private readonly IEnumerable<IAIToolProvider> _toolProviders;

        public AIToolRegistry(IEnumerable<IAIToolProvider> toolsProvided)
        {
            _toolProviders = toolsProvided;
        }

        public List<AITool> GetTools()
        {
            var buildingTools = new List<AITool>();

            foreach (var provider in _toolProviders)
            {
                //il servizio mettendolo come AITool
                var toolMethods = provider.GetType().GetMethods().Where(el => el.GetCustomAttributesData().Any(a => a.AttributeType == typeof(AIToolAttribute))).ToList();

                toolMethods.ForEach(m => buildingTools.Add(AIFunctionFactory.Create(m, provider)));
            }

            return buildingTools;
        }
    }
}
