using DotNetAIAgent.Evaluation.Model;
using DotNetAIAgent.Generation;
using DotNetAIAgent.Model;
using System.Data;

namespace DotNetAIAgent.Evaluation.Services;

public class RetrieverEvaluator
{
    private readonly RagRetriever _ragRetriever;

    public RetrieverEvaluator(RagRetriever ragRetriever)
    {
        _ragRetriever = ragRetriever;
    }

    /// <summary>
    /// Il retrieve va a recuperare i top-k elementni dalla nostra collezione di vectors che rappresentano il document.
    /// Il metodo di RetrieveWithScores ritorna i vector con un valore di similarity
    /// </summary>
    /// <param name="queryEvaluation"></param>
    /// <param name="indexedDocumentChunks"></param>
    /// <returns></returns>
    public async Task<RetrievalEvaluationOutcome> RetrieveOutcome(RetrievalEvaluationCase queryEvaluation, IEnumerable<IndexedDocumentChunk> indexedDocumentChunks, int topK)
    { 
        if (!indexedDocumentChunks.Any())
        { 
            return new RetrievalEvaluationOutcome(queryEvaluation, new List<SearchResultEmbedding>() { });;
        }

        var searchResult = (await _ragRetriever.RetrieveWithScoresAsync(indexedDocumentChunks, queryEvaluation.Query, topK)).ToList();

        return new RetrievalEvaluationOutcome(queryEvaluation, searchResult);
    }

    public bool EvaluateOutcome(RetrievalEvaluationOutcome outcomeRetrieve)
    { 
        if (!outcomeRetrieve.ResultingEmbedding.Any())
        { 
            return false;
        }

        var expectingChunkId = outcomeRetrieve.EvaluationRetrieval.ExpectedChunkId;

        var outcomingChunk = outcomeRetrieve.ResultingEmbedding.First();
        var chunkId = outcomingChunk.IndexedDocument.DocumentChunk.ChunkId;

        return chunkId == expectingChunkId;
    }

    public bool EvaluateHitAtK(RetrievalEvaluationOutcome outcomeRetrieve, int k)
    {
        if (!outcomeRetrieve.ResultingEmbedding.Any() || k == 0)
        { 
            return false;
        }

        var expectedChunk = outcomeRetrieve.EvaluationRetrieval.ExpectedChunkId;

        return outcomeRetrieve.ResultingEmbedding.Take(k).Any(el => expectedChunk == el.IndexedDocument.DocumentChunk.ChunkId);
    }
}
