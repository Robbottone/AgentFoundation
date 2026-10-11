namespace DotNetAIAgent.Evaluation.Model;

public record RetrievalBenchmarkResult(int TotalCases, int CorrectOutcome, float TopOneAccuracy, float CorrectHitAtK);
