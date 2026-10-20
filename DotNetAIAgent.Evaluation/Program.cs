using DotNetAIAgent;
using DotNetAIAgent.Embedding;
using DotNetAIAgent.Evaluation;
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

var entireKnowledge = KnowledgeBaseLoader.LoadKnowledgeBase();
var knowledgeBaseService = scope.GetRequiredService<KnowledgeBaseEmbeddingService>();

var documentService = scope.GetRequiredService<DocumentChunkingService>();
var retrievalDatabaseLoader = scope.GetRequiredService<RetrievalDatasetLoader>();
var retrieverBanchmark = scope.GetRequiredService<RetrievalBenchmark>();
var ragRetriever = scope.GetRequiredService<RagRetriever>();
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

//creo gli embedding del document
var embeddings = await Task.WhenAll(
                            entireKnowledge.Select(kb =>
                                knowledgeBaseService.CreateEmbeddingsAsync(kb)).ToList());

//select-many per appiattire gli embeddings su unico vettore. I documenti vengono riconosciuto per documentIds
var embs = embeddings.SelectMany(x => x).ToList();

var hitAt = 3;

foreach(var dataset in datasets)
{ 
    var outcomes = await retrieverBanchmark.ExecuteBenchmark(dataset.Cases, embs, hitAt);

    Console.WriteLine($"Total cases = {outcomes.TotalCases}");
    Console.WriteLine($"TopOne accuracy = {outcomes.TopOneAccuracy}");
    Console.WriteLine($"CORRECT OUTCOMES = {outcomes.CorrectOutcome}");
    Console.WriteLine($"Hit at {hitAt} = {outcomes.CorrectHitAtK}");
}
