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
using Microsoft.Extensions.Options;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
services.AddAgentFoundation(configuration);

var serviceProvider = services.BuildServiceProvider();

var agentApplication =  serviceProvider.GetRequiredService<AgentApplication>();

await agentApplication.RunAsync();

