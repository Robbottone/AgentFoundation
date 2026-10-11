using DotNetAIAgent.Evaluation.Model;
using DotNetAIAgent.Model;

namespace DotNetAIAgent.Evaluation.Services;

public class RetrievalBenchmark
{
    private readonly RetrieverEvaluator _retrieverEvaluator;

    public RetrievalBenchmark(RetrieverEvaluator retrieverEvaluator)
    {
        _retrieverEvaluator = retrieverEvaluator;
    }

    public async Task<RetrievalBenchmarkResult> ExecuteBenchmark(
        IEnumerable<RetrievalEvaluationCase> cases,
        IEnumerable<IndexedDocumentChunk> indexedChunks,
        int atK)
    { 
        if (!cases.Any() || !indexedChunks.Any())
            return new RetrievalBenchmarkResult(cases.Count(), 0, 0f, 0);

        var correctOutcome = 0;

        var casesCount = cases.Count();
        var hitAtK = 0;

        foreach (var query in cases)
        {
            var outcome = await _retrieverEvaluator.RetrieveOutcome(query, indexedChunks, atK);

            if (_retrieverEvaluator.EvaluateOutcome(outcome))
            { 
                correctOutcome++;
            }

            if (_retrieverEvaluator.EvaluateHitAtK(outcome, atK))
            { 
                hitAtK++;
            }
        }

        var resultTopOne = (float)correctOutcome/casesCount;
        var resultHitAtK = (float)hitAtK/casesCount;

        return new RetrievalBenchmarkResult(cases.Count(), correctOutcome, resultTopOne, resultHitAtK);
    }
}
