using DotNetAIAgent.Model;

namespace DotNetAIAgent.Evaluation.Model;

public record RetrievalEvaluationOutcome(RetrievalEvaluationCase EvaluationRetrieval, IEnumerable<SearchResultEmbedding> ResultingEmbedding);

