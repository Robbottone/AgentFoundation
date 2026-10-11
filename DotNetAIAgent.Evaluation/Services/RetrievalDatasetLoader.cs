using DotNetAIAgent.Evaluation.Model;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetAIAgent.Evaluation.Services;

public class RetrievalDatasetLoader
{
    public IEnumerable<RetrievalEvaluationDataset> LoadDatasets()
    { 
        var assetsFolder = Path.Combine(AppContext.BaseDirectory, "RetrievalAssets");

        if (!Directory.Exists(assetsFolder))
        { 
            throw new InvalidOperationException("Cannot proceed without the assets directory to load the retrieval cases");
        }

        var jsonPaths = Directory.GetFiles(assetsFolder, "*.json");

        if (jsonPaths.Length == 0)
        { 
            throw new InvalidOperationException($"Cannot proceed, no JSON files in the following folder {assetsFolder}"); 
        }

        var dataSetResult = new List<RetrievalEvaluationDataset>();
        var jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        foreach( var jsonPath in jsonPaths)
        {
            string jsonString = File.ReadAllText(jsonPath);

            var dataset = JsonSerializer.Deserialize<RetrievalEvaluationDataset>(jsonString, jsonOptions);
            
            if (dataset is null || !dataset.Cases.Any()) continue;

            dataSetResult.Add(dataset);
        }

        return dataSetResult;
    }
}
