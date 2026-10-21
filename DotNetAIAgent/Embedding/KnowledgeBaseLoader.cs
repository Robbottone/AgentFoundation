using DotNetAIAgent.Model;
using System.Text.Json;

namespace DotNetAIAgent.Embedding;

public static class KnowledgeBaseLoader
{
    private const string documentFileName = "documentIds.json";

    public static IEnumerable<KnowledgeDocument> LoadKnowledgeBase()
    {
        var knowledgeFolder = Path.Combine(AppContext.BaseDirectory, "Knowledge");
        var knowledgeCollection = new List<KnowledgeDocument>();

        if (!Directory.Exists(knowledgeFolder))
            throw new ArgumentNullException($"La folder non esiste: {knowledgeFolder}");

        //read file-ids
        var mappingDocumentPath = Path.Combine(knowledgeFolder, documentFileName);

        if (!File.Exists(mappingDocumentPath))
            throw new FileNotFoundException("Mappatura documenti non trovata", mappingDocumentPath);

        var knowledgeFiles = Directory.GetFiles(knowledgeFolder,"knowledge-base-*.txt");

        var documentIds = JsonSerializer.Deserialize<Dictionary<string, Guid>>(File.ReadAllText(mappingDocumentPath)) ?? throw new InvalidDataException("Mappatura documenti non valida");

        foreach(var file in knowledgeFiles)
        {
            var fileInfo = new FileInfo(file);
            
            var readFile = File.ReadAllText(file);

            if (string.IsNullOrWhiteSpace(readFile)) throw new ArgumentNullException(nameof(readFile), "File vuoto o non raggiungibile");

            if (!documentIds.TryGetValue(fileInfo.Name, out var fileguid))
            {
                throw new InvalidDataException($"DocumentId mancante per il documento: {fileInfo.Name}");
            }

            var knowledgeBase = new KnowledgeDocument(fileguid, fileInfo.Name, readFile);

            knowledgeCollection.Add(knowledgeBase);
        }

        return knowledgeCollection;
    }
}
