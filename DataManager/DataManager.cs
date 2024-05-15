using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using Models;

namespace DataManager;

public static class DataManager<T> where T : new()
{
    public static async Task<List<T>> ImportCsvAsync<T>(string filePath, ClassMap<T> map) where T : class, new()
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture)
               {
                   MissingFieldFound = null,
                   HeaderValidated = null,
                   IgnoreReferences = true
               }))
        {
            csv.Context.RegisterClassMap(map);
            var records = new List<T>();
            await Task.Run(() => records = csv.GetRecords<T>().ToList());
            return records;
        }
    }




    public static Task<List<T>> ImportXmlAsync(string filePath)
    {
        var serializer = new XmlSerializer(typeof(List<T>));
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            var result = (List<T>) serializer.Deserialize(fileStream)!;
            return Task.FromResult(result);
        }
    }

    public static Task ExportXmlAsync(List<T> items, string filePath)
    {
        var serializer = new XmlSerializer(typeof(List<T>));
        using (TextWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, items);
        }

        return Task.CompletedTask;
    }


    public static async Task<List<T>?> ImportJsonAsync(string filePath)
    {
        var jsonString = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<T>>(jsonString);
    }

    public static async Task ExportJsonAsync(List<T> items, string filePath)
    {
        var options = new JsonSerializerOptions {WriteIndented = true};
        var jsonString = JsonSerializer.Serialize(items, options);
        await File.WriteAllTextAsync(filePath, jsonString);
    }
}