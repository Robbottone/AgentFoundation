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

    public async Task<RetrievalEvaluationOutcome> RetrieveOutcome(RetrievalEvaluationCase queryEvaluation, IEnumerable<IndexedDocumentChunk> indexedDocumentChunks)
    { 
        if (!indexedDocumentChunks.Any())
        { 
            return new RetrievalEvaluationOutcome(queryEvaluation, new List<SearchResultEmbedding>() { });;
        }

        var searchResult = (await _ragRetriever.RetrieveWithScoresAsync(indexedDocumentChunks, queryEvaluation.Query)).ToList();

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
        
        return outcomingChunk.IndexedDocument.DocumentChunk.ChunkId == expectingChunkId;
    }
}
