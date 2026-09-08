using BasicHelloWorld.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BasicHelloWorld.Tools
{
    public static class AIToolServiceCollectionExtensions
    {
        public static void RegisterAIToolProviders(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var compatibleType = assembly.GetTypes()
                                         .Where(el =>
                                            el.IsClass && !el.IsAbstract && typeof(IAIToolProvider).IsAssignableFrom(el))
                                         .ToList();

            foreach (var type in compatibleType)
            {
                services.AddScoped(typeof(IAIToolProvider), type);
            }
        }
    }
}
