using DotNetAIAgent;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
services.AddAgentFoundation(configuration);

var serviceProvider = services.BuildServiceProvider();

var agentApplication =  serviceProvider.GetRequiredService<AgentApplication>();

await agentApplication.RunAsync();

