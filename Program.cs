using BasicHelloWorld;
using BasicHelloWorld.Factory;
using BasicHelloWorld.Services;
using BasicHelloWorld.Tools;
using DotNetAIAgent.Embedding;
using DotNetAIAgent.Interface;
using DotNetAIAgent.Model;
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

services.RegisterAIToolProviders();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var chatClient = scope.ServiceProvider.GetRequiredService<IChatClient>();

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = scope.ServiceProvider
                                                                        .GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

var texts = new List<string>
{
    "Il cane dorme sul divano",
    "Il mio cucciolo sta riposando sul sofà",
    "C# supporta la programmazione asincrona",
};

var query = new List<string>
{
    "Dove sta riposando l'animale?"
};

var embeddingsText = await embeddingGenerator.GenerateAsync(texts);
var embeddingsQuery = await embeddingGenerator.GenerateAsync(query);

var indexedTexts = embeddingsText.Select((el, idx) => new IndexedText(texts[idx], el.Vector)).ToList() ?? new List<IndexedText>();
var vectorQuery = embeddingsQuery.First();

Console.WriteLine($"Query: {query.First()}");

var vectorSearch = new VectorSearchService();
var indexedTextResult = vectorSearch.Search(vectorQuery.Vector, indexedTexts);

var count = 1;
foreach (var item in indexedTextResult)
{
    Console.WriteLine($"Position {count}");
    Console.WriteLine($"Text: {item.IndexedText.Text} and cosine similarity {item.Similarity}");
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