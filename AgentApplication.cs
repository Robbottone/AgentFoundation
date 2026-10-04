using DotNetAIAgent.Embedding;
using DotNetAIAgent.Generation;
using DotNetAIAgent.Model;
using Microsoft.Extensions.AI;

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

            var messages = new List<ChatMessage>();

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
                var queryStandalone = await _chatQueryRewriter.RewriteQueryAsync(query, messages);
                var documentChunkResult = await _ragRetriever.RetrieveAsync(chunks, queryStandalone);
                #endregion

                #region Rag Input Message

                var ragMessage = _ragMessageBuilder.CreateRagMessage(documentChunkResult, query);

                ChatMessage message = new ChatMessage(ChatRole.User, ragMessage);
                messages.Add(message);
                #endregion

                #region Message Response
                var response = await _chatResponseService.GenerateResponseAsync(messages, _chatOptions);
                Console.WriteLine(response.Text);

                if (response.FinishReason.HasValue && response.FinishReason.Value == ChatFinishReason.Length)
                {
                    Console.WriteLine("[La risposta è stata interrotta perché è stato raggiunto il limite massimo di token.]");
                }

                #endregion

                #region Token Report
                Console.WriteLine();
                Console.WriteLine($"Token utilizzati in totale: \n input: {response.InputTokens} | output: {response.OutputTokens} \n totale: {response.InputTokens + response.OutputTokens}");

                var responseChat = new ChatMessage(ChatRole.Assistant, response.Text);
                #endregion

                #region History Message
                messages.Add(responseChat);
                #endregion
            }
        }
    }
}
