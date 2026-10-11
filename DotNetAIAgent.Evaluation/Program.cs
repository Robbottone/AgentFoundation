using DotNetAIAgent;
using DotNetAIAgent.Embedding;
using DotNetAIAgent.Evaluation;
using DotNetAIAgent.Evaluation.Model;
using DotNetAIAgent.Evaluation.Services;
using DotNetAIAgent.Generation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
services.AddAgentFoundation(configuration);
services.AddEvaluationServices();

var serviceProvider = services.BuildServiceProvider();
var serviceScope = serviceProvider.CreateScope();

var scope = serviceScope.ServiceProvider;

KnowledgeBaseEmbeddingService knowledgeBaseService = scope.GetRequiredService<KnowledgeBaseEmbeddingService>();

var documentService = scope.GetRequiredService<DocumentChunkingService>();
var retrievalDatabaseLoader = scope.GetRequiredService<RetrievalDatasetLoader>();
var retrieverBanchmark = scope.GetRequiredService<RetrievalBenchmark>();
var ragRetriever = scope.GetRequiredService<RagRetriever>();

var knowledegeBase = AgentApplication.LoadKnowledgeBase();

var documentChunked = documentService.GenerateDocumentChunk(knowledegeBase, 400, 80);
var datasets = retrievalDatabaseLoader.LoadDatasets();

#region stampatore
//var counter = 1;

//foreach (var chunk in documentChunked)
//{ 
//   Console.WriteLine($"======== Chunk {counter} ========");
//   Console.WriteLine($"Document Id: {chunk.ChunkId}");
//   Console.WriteLine();
//   Console.WriteLine(chunk.ChunkText);
//   Console.WriteLine();
//   Console.WriteLine("==================================");

//   counter++;
//}
#endregion

var embeddings = (await knowledgeBaseService.CreateEmbeddingsAsync(knowledegeBase)).ToList();

if (!embeddings.Any() || !datasets.Any())
    return;

var retrievalCases = datasets.First(el => el.DocumentId == Guid.Parse("a13c7e52-2b60-4d71-9f84-31e24dcb8760")).Cases;

var hitAt = 3;

var outcomes = await retrieverBanchmark.ExecuteBenchmark(retrievalCases, embeddings, hitAt);

Console.WriteLine($"Total cases = {outcomes.TotalCases}");
Console.WriteLine($"TopOne accuracy = {outcomes.TopOneAccuracy}");
Console.WriteLine($"CORRECT OUTCOMES = {outcomes.CorrectOutcome}");
Console.WriteLine($"Hit at {hitAt} = {outcomes.CorrectHitAtK}");
