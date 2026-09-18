using DotNetAIAgent;
using DotNetAIAgent.Embedding;
using DotNetAIAgent.Factory;
using DotNetAIAgent.Interface;
using DotNetAIAgent.Model;
using DotNetAIAgent.Services;
using DotNetAIAgent.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using System.Text;
using System.Text.Json;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

//options -> mappa settings tipizzata
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

services.RegisterAIToolProviders();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var chatClient = scope.ServiceProvider.GetRequiredService<IChatClient>();

var documentChunkService = scope.ServiceProvider.GetRequiredService<DocumentChunkingService>();
var indexedDocumentChunkService = scope.ServiceProvider.GetRequiredService<DocumentChunkIndexService>();

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = scope.ServiceProvider
                                                                        .GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

var fileName = Path.Combine(AppContext.BaseDirectory,"Knowledge","knowledge-base-thermohome-x200.txt");

if (!File.Exists(fileName))
    throw new ArgumentNullException("File non esiste");

var readFile = File.ReadAllText(fileName);

if (string.IsNullOrEmpty(readFile)) throw new ArgumentNullException(nameof(readFile), "File vuoto o non raggiungibile");

var fileGuid = Guid.NewGuid();
var fileGuidTest = Guid.NewGuid();
var knowledgeBase = new KnowledgeDocument(fileGuid, fileName, readFile);

var documentChunks = documentChunkService.GenerateDocumentChunk(knowledgeBase, 400, 80);

var indexedDocumentChunks = indexedDocumentChunkService.GenerateDocumentChunkIndex(documentChunks);
List<IndexedDocumentChunk> chunks = new(); 

await foreach(var indChunk in indexedDocumentChunks)
{
    chunks.Add(indChunk);
}

var query = new List<string>
{
    "La caldaia mi dà errore E15, cosa devo fare?"
};

var embeddingsQuery = await embeddingGenerator.GenerateAsync(query);
var vectorQuery = embeddingsQuery.First();

Console.WriteLine($"Query: {query.First()}\n");

var vectorSearch = new VectorSearchService();
var indexedTextResult = vectorSearch.Search(vectorQuery.Vector, chunks);

var count = 1;

foreach (var item in indexedTextResult)
{
    Console.WriteLine($"========== RESULT #{count} ==========");
    Console.WriteLine($"Similarity {item.Similarity}");
    Console.WriteLine($"Start index {item.IndexedDocument.DocumentChunk.StartIndex}\n");

    Console.WriteLine($"{item.IndexedDocument.DocumentChunk.ChunkText}\n");
    count++;
}

Console.WriteLine("Hi! How can I help you?");

var messages = new List<ChatMessage>();
var options = scope.ServiceProvider.GetRequiredService<ChatOptions>();

while (true)
{
    var requestMessage = Console.ReadLine();
    bool limitWarningShown = false;

    ChatMessage message = new ChatMessage(ChatRole.User, requestMessage);

    messages.Add(message);

    var responses = chatClient.GetStreamingResponseAsync(messages, options);

    StringBuilder sb = new StringBuilder();
    List<(int inputToken, int outputToken)> tokenInputOutput = new();

    await foreach (var item in responses)
    {
        if (!string.IsNullOrEmpty(item.Text))
        {
            sb.Append(item.Text);
        }

        if (item.Contents.Any())
        {
            foreach (var content in item.Contents)
            {
                Console.WriteLine(content.GetType().Name);

                if (content is FunctionCallContent functionCall)
                {
                    Console.WriteLine(functionCall.Name);
                    Console.WriteLine(JsonSerializer.Serialize(functionCall.Arguments));
                }
            }
        }

        if (item.RawRepresentation is StreamingChatCompletionUpdate metaDataRawChatUpdate)
        {
            if (metaDataRawChatUpdate?.Usage is not null)
            {
                Console.WriteLine($"Usage: {metaDataRawChatUpdate.Usage.TotalTokenCount}");
                tokenInputOutput.Add(new(metaDataRawChatUpdate.Usage.InputTokenCount,
                                         metaDataRawChatUpdate.Usage.OutputTokenCount));
            }
        }
       
        if (item.FinishReason == Microsoft.Extensions.AI.ChatFinishReason.Length && !limitWarningShown)
        {
            Console.WriteLine($"[La risposta è stata interrotta perché è stato raggiunto il limite di token]");
            limitWarningShown = true;
        }
    }

    var inputTokenSum = 0;
    var outputTokenSum = 0;
    tokenInputOutput.ForEach(el => {inputTokenSum += (el.inputToken); outputTokenSum += el.outputToken;});

    Console.WriteLine(sb.ToString());

    Console.WriteLine($"Token utilizzati in totale: \n input: {inputTokenSum} | output: {outputTokenSum} \n totale: {inputTokenSum+outputTokenSum}");

    var responseChat = new ChatMessage(ChatRole.Assistant, sb.ToString());

    messages.Add(responseChat);
}