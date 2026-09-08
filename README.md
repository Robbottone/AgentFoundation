# .NET AI Agent

A small .NET project built to explore how LLM-based applications can be
structured beyond a basic chat completion example.

The project uses Azure OpenAI through `Microsoft.Extensions.AI` and implements
a conversational agent capable of streaming responses and invoking application
tools.

## What it currently does

The agent can:

- Maintain a conversation history.
- Stream responses from an Azure OpenAI model.
- Expose C# methods as tools that can be selected and invoked by the LLM.
- Automatically discover tool providers using reflection.
- Execute multiple tools when required to answer a request.
- Track input and output token usage.

The sample tools currently provide product information such as price and
availability.

## Architecture

The project separates the main responsibilities of the agent:

- **ChatClientFactory** creates and configures the `IChatClient`.
- **AgentChatOptionsFactory** builds the `ChatOptions` used by the agent.
- **AIToolRegistry** discovers the available AI tools.
- **IAIToolProvider** identifies classes that expose methods to the LLM.
- **ProductTools** exposes AI-callable operations.
- **ProductServices** contains the underlying application/business logic.

Dependencies are managed using `Microsoft.Extensions.DependencyInjection`.

Application configuration is loaded from `appsettings.json` and mapped to
strongly typed options using the .NET Options pattern.

## Tool calling

Tool providers implement `IAIToolProvider`.

Methods exposed to the model are marked with a custom `AITool` attribute.
At startup, the application discovers these methods and converts them into
AI functions using `AIFunctionFactory`.

This allows new tool providers to be added without manually registering each
individual tool in the chat configuration.

## Technologies

- .NET
- C#
- Azure OpenAI
- Microsoft.Extensions.AI
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Options

## Current status

This is an evolving learning project focused on understanding the architecture
behind LLM applications rather than hiding the implementation behind
high-level frameworks.

Current milestone:

`Conversational Agent + Streaming + Function Calling + Dependency Injection`

Next steps will explore:

`Embeddings → Vector Search → Retrieval-Augmented Generation (RAG)`
