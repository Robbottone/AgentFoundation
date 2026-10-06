using DotNetAIAgent.Embedding;
using DotNetAIAgent.Generation;
using DotNetAIAgent.Model;
using Microsoft.Extensions.AI;
using System.Text;

namespace DotNetAIAgent
{
    public class AgentApplication
    {
        private readonly KnowledgeBaseEmbeddingService _knowledgeEmbeddingService;
        private readonly RagMessageBuilder _ragMessageBuilder;
        private readonly RagRetriever _ragRetriever;
        private readonly ChatResponseService _chatResponseService;
        private readonly ChatQueryRewriter _chatQueryRewriter;
        private readonly ChatOptions _chatOptions;

        public AgentApplication(KnowledgeBaseEmbeddingService knowledgeBaseEmbedding,
                                RagMessageBuilder ragMessageBuilder,
                                RagRetriever ragRetriever,
                                ChatResponseService chatResponseService,
                                ChatQueryRewriter chatQueryRewriter,
                                ChatOptions chatOptions)
        {
            _knowledgeEmbeddingService = knowledgeBaseEmbedding;
            _ragMessageBuilder = ragMessageBuilder;
            _ragRetriever = ragRetriever;
            _chatResponseService = chatResponseService;
            _chatQueryRewriter = chatQueryRewriter;
            _chatOptions = chatOptions;
        }

        public async Task RunAsync()
        {
            var fileName = Path.Combine(AppContext.BaseDirectory, "Knowledge", "knowledge-base-thermohome-x200.txt");

            if (!File.Exists(fileName))
                throw new ArgumentNullException("File non esiste");

            var readFile = File.ReadAllText(fileName);

            if (string.IsNullOrEmpty(readFile)) throw new ArgumentNullException(nameof(readFile), "File vuoto o non raggiungibile");

            var fileGuid = Guid.NewGuid();

            var knowledgeBase = new KnowledgeDocument(fileGuid, fileName, readFile);
            
            var chunks = await _knowledgeEmbeddingService.CreateEmbeddingsAsync(knowledgeBase);

            var historyOfMessages = new List<ChatMessage>();

            while (true)
            {
                #region Query Ingestion
                Console.WriteLine();
                Console.WriteLine("Make a request..");
                var query = Console.ReadLine();

                if (string.IsNullOrEmpty(query))
                    continue;
                #endregion

                #region Retriever
                var queryStandalone = await _chatQueryRewriter.RewriteQueryAsync(query, historyOfMessages);
                var documentChunkResult = (await _ragRetriever.RetrieveAsync(chunks, queryStandalone)).ToList();
                #endregion

                #region Rag Input Message

                if (!documentChunkResult.Any())
                { 
                    Console.WriteLine("Non ho trovato informazioni sufficientemente rilevanti nella knowledge base");
                    continue;
                }

                var ragMessage = _ragMessageBuilder.CreateRagMessage(documentChunkResult, queryStandalone);

                ChatMessage message = new ChatMessage(ChatRole.User, ragMessage.ToString());
                #endregion

                #region Message Response
                var response = await _chatResponseService.GenerateResponseAsync(historyOfMessages.Append(message).ToList(), _chatOptions);

                Console.WriteLine("Response:");
                Console.WriteLine();
                Console.WriteLine(response.Text);

                if (response.FinishReason.HasValue && response.FinishReason.Value == ChatFinishReason.Length)
                {
                    Console.WriteLine("[La risposta è stata interrotta perché è stato raggiunto il limite massimo di token.]");
                }

                #endregion

                #region Token Report
                Console.WriteLine();
                Console.WriteLine($"Token utilizzati in totale: \n input: {response.InputTokens} | output: {response.OutputTokens} \n totale: {response.InputTokens + response.OutputTokens}");

                var queryChat = new ChatMessage(ChatRole.User, query);
                var responseChat = new ChatMessage(ChatRole.Assistant, response.Text);
                #endregion

                #region History Message
                historyOfMessages.AddRange(queryChat, responseChat);
                #endregion
            }
        }
    }
}
