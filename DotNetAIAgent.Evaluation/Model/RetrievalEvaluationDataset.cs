namespace DotNetAIAgent.Evaluation.Model;
public record RetrievalEvaluationDataset( Guid DocumentId, string Name, IEnumerable<RetrievalEvaluationCase> Cases);