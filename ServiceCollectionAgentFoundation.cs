using DotNetAIAgent.Embedding;
using DotNetAIAgent.Factory;
using DotNetAIAgent.Generation;
using DotNetAIAgent.Interface;
using DotNetAIAgent.Model;
using DotNetAIAgent.Services;
using DotNetAIAgent.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetAIAgent
{
    public static class ServiceCollectionAgentFoundation
    {
        public static IServiceCollection AddAgentFoundation(this IServiceCollection services, IConfiguration configuration)
        { 
            //options with type
            services.Configure<AgentConnectionOptions>(configuration.GetSection("AgentConnection"));

            services.AddScoped<IChatClientFactory, ChatClientFactory>();
            services.AddScoped<IChatOptionsFactory, AgentChatOptionsFactory>();
            services.AddScoped<IEmbeddingClientFactory, EmbeddingClientFactory>();
            services.AddScoped(sp => sp.GetRequiredService<IChatClientFactory>().Create());
            services.AddScoped(sp => sp.GetRequiredService<IChatOptionsFactory>().Create());
            services.AddScoped(sp => sp.GetRequiredService<IEmbeddingClientFactory>().Create());
            services.AddScoped<ProductServices>();
            services.AddScoped<AIToolRegistry>();
            services.AddScoped<DocumentChunkingService>();
            services.AddScoped<DocumentChunkIndexService>();
            services.AddScoped<RagMessageBuilder>();
            services.AddScoped<VectorSearchService>();
            services.AddScoped<RagRetriever>();
            services.AddScoped<ChatResponseService>();

            services.RegisterAIToolProviders();

            services.AddScoped<AgentApplication>();

            return services;
        }
    }
}
