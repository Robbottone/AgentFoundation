using DotNetAIAgent.Evaluation.Services;

using Microsoft.Extensions.DependencyInjection;

namespace DotNetAIAgent.Evaluation;

public static class ServiceCollectionAgentFoundationEvaluation
{
    public static IServiceCollection AddEvaluationServices(this IServiceCollection services)
    { 
        services.AddScoped<RetrieverEvaluator>();
        services.AddScoped<RetrievalBenchmark>();
        services.AddScoped<RetrievalDatasetLoader>();

        return services;
    }
}
