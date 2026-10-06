using DotNetAIAgent.Evaluation.Model;

namespace DotNetAIAgent.Evaluation.Services;

public class RetrievalBenchmark
{
    private readonly RetrieverEvaluator _retrieverEvaluator;

    public RetrievalBenchmark(RetrieverEvaluator retrieverEvaluator)
    {
        _retrieverEvaluator = retrieverEvaluator;
    }

    public Task<RetrievalEvaluationOutcome> ExecuteBenchmark(IEnumerable<RetrievalEvaluationCase> cases)
    { 
        throw new NotImplementedException();
    }
}
