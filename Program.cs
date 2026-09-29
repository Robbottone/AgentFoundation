using DotNetAIAgent;
using DotNetAIAgent.Embedding;
using DotNetAIAgent.Factory;
using DotNetAIAgent.Generation;
using DotNetAIAgent.Interface;
using DotNetAIAgent.Model;
using DotNetAIAgent.Services;
using DotNetAIAgent.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using System.Text;
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
services.AddScoped<RagMessageBuilder>();
services.AddScoped<VectorSearchService>();
services.AddScoped<RagRetriever>();

services.RegisterAIToolProviders();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var chatClient = scope.ServiceProvider.GetRequiredService<IChatClient>();

var documentChunkService = scope.ServiceProvider.GetRequiredService<DocumentChunkingService>();
var indexedDocumentChunkService = scope.ServiceProvider.GetRequiredService<DocumentChunkIndexService>();
var ragMessageBuilder = scope.ServiceProvider.GetRequiredService<RagMessageBuilder>();
var ragRetriever = scope.ServiceProvider.GetRequiredService<RagRetriever>();

var fileName = Path.Combine(AppContext.BaseDirectory,"Knowledge","knowledge-base-thermohome-x200.txt");

if (!File.Exists(fileName))
    throw new ArgumentNullException("File non esiste");

var readFile = File.ReadAllText(fileName);

if (string.IsNullOrEmpty(readFile)) throw new ArgumentNullException(nameof(readFile), "File vuoto o non raggiungibile");

var fileGuid = Guid.NewGuid();

var knowledgeBase = new KnowledgeDocument(fileGuid, fileName, readFile);
var documentChunks = documentChunkService.GenerateDocumentChunk(knowledgeBase, 400, 80);
var indexedDocumentChunks = indexedDocumentChunkService.GenerateDocumentChunkIndex(documentChunks);

List<IndexedDocumentChunk> chunks = new();  

await foreach(var indChunk in indexedDocumentChunks)
{
    chunks.Add(indChunk);
}

var messages = new List<ChatMessage>();
var options = scope.ServiceProvider.GetRequiredService<ChatOptions>();

while(true)
{
    #region Query Ingestion
    Console.WriteLine();
    Console.WriteLine("Make a request..");
    var query = Console.ReadLine();

    if (string.IsNullOrEmpty(query))
        continue;
    #endregion

    #region Retriever
    var documentChunkResult = await ragRetriever.RetrieveAsync(chunks, query);
    #endregion

    #region Rag Input Message

    var ragMessage = ragMessageBuilder.CreateRagMessage(documentChunks, query);
    
    bool limitWarningShown = false;

    ChatMessage message = new ChatMessage(ChatRole.User, ragMessage);

    messages.Add(message);
    #endregion

    #region Message Response
    var responses = chatClient.GetStreamingResponseAsync(messages, options);

    StringBuilder sb = new StringBuilder();
    List<(int inputToken, int outputToken)> tokenInputOutput = new();

    await foreach (var item in responses)
    {
        if (!string.IsNullOrEmpty(item.Text))
        {
            sb.Append(item.Text);
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
    #endregion

    #region Token Report
    var inputTokenSum = 0;
    var outputTokenSum = 0;
    tokenInputOutput.ForEach(el => {inputTokenSum += (el.inputToken); outputTokenSum += el.outputToken;});

    Console.WriteLine(sb.ToString());

    Console.WriteLine($"Token utilizzati in totale: \n input: {inputTokenSum} | output: {outputTokenSum} \n totale: {inputTokenSum+outputTokenSum}");

    var responseChat = new ChatMessage(ChatRole.Assistant, sb.ToString());
    #endregion

    #region History Message
    messages.Add(responseChat);
    #endregion
}