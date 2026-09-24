AgentFoundation
A .NET learning project for exploring how production-oriented LLM applications can be designed and implemented from the ground up.
The project uses Azure OpenAI through `Microsoft.Extensions.AI` and progressively builds the foundations of an AI agent and Retrieval-Augmented Generation (RAG) system without hiding the core concepts behind high-level frameworks.
The goal is to understand how the individual components work and how they fit together in a real application architecture.
Current capabilities
Conversational Agent
Maintain conversation history.
Stream responses from Azure OpenAI.
Track input and output token usage.
Configure the agent through strongly typed options.
Function Calling
Expose C# methods as tools callable by the LLM.
Automatically discover tool providers using reflection.
Execute multiple tools when required.
Separate AI-callable tools from the underlying business logic.
Tool providers implement `IAIToolProvider`. Methods exposed to the model are marked with a custom `AITool` attribute and converted into AI functions using `AIFunctionFactory`.
Knowledge Ingestion
Documents can be transformed into smaller retrieval units before being indexed.
The current ingestion pipeline supports:
`KnowledgeDocument` as the source document representation.
`DocumentChunk` for individual retrieval units.
Paragraph-aware chunking.
Sentence splitting for oversized paragraphs.
Character-based fallback for oversized sentences.
Source position tracking through `StartIndex`.
The chunking strategy attempts to preserve semantic boundaries instead of splitting documents only at fixed character positions.
Embeddings and Indexing
Document chunks are converted into vector representations using Azure OpenAI embeddings.
Each indexed chunk is represented by an `IndexedDocumentChunk`, which associates the original `DocumentChunk` with its embedding vector.
The current implementation uses an in-memory vector store to keep the retrieval pipeline explicit and easy to understand.
Semantic Vector Search
Queries are converted into embeddings and compared against indexed document chunks.
The current search pipeline implements:
Query embedding generation.
Cosine similarity.
Brute-force vector comparison.
Similarity ranking.
Top-K retrieval.
This implementation intentionally performs the similarity calculation directly instead of relying on a vector database, allowing the underlying retrieval mechanics to remain visible.
Retrieval pipeline
```text
Knowledge Document
       ↓
Paragraph-aware Chunking
       ↓
DocumentChunk[]
       ↓
Embedding Generation
       ↓
IndexedDocumentChunk[]
       ↓
In-memory Vector Store

User Query
       ↓
Query Embedding
       ↓
Cosine Similarity
       ↓
Ranking
       ↓
Top-K Relevant Chunks
```
Architecture
The project separates the main responsibilities of the application.
ChatClientFactory creates and configures the `IChatClient`.
AgentChatOptionsFactory builds the `ChatOptions` used by the agent.
AIToolRegistry discovers available AI tools.
IAIToolProvider identifies classes exposing methods to the LLM.
ProductTools exposes AI-callable application operations.
ProductServices contains the underlying application/business logic.
DocumentChunkingService transforms knowledge documents into retrieval chunks.
DocumentChunkIndexService generates embeddings for document chunks.
IndexedDocumentChunk represents an indexed chunk and its vector.
Dependencies are managed using `Microsoft.Extensions.DependencyInjection`.
Application configuration is loaded from `appsettings.json` and mapped to strongly typed options using the .NET Options pattern.


The project currently implements the complete retrieval side of a basic RAG pipeline:
`Document → Chunking → Embeddings → Vector Search → Top-K`
Development is now focused on connecting retrieved knowledge to the LLM:
`Top-K → Context Construction → Grounded Generation → Citations`
Learning approach
AgentFoundation intentionally implements several concepts at a lower level before introducing specialized infrastructure such as vector databases or higher-level RAG frameworks.
The purpose is not to recreate those systems for production use, but to understand the mechanics they abstract away and the architectural decisions involved in building LLM applications.
