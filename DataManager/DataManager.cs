using System.Globalization;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataManager;

/// <summary>
///     Bietet Methoden zum Importieren und Exportieren von Daten in verschiedenen Formaten wie CSV, XML und JSON.
/// </summary>
public static class DataManager<T> where T : new()
{
    /// <summary>
    ///     Importiert Daten aus einer CSV-Datei in eine Liste von Objekten des Typs T.
    /// </summary>
    /// <param name="filePath">Der Pfad zur CSV-Datei.</param>
    /// <param name="map">Das ClassMap-Objekt, das verwendet wird, um die CSV-Daten den Eigenschaften von T zuzuordnen.</param>
    /// <returns>Eine Liste von Objekten des Typs T.</returns>
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

    /// <summary>
    ///     Importiert Daten aus einer XML-Datei in eine Liste von Objekten des Typs T.
    /// </summary>
    /// <param name="filePath">Der Pfad zur XML-Datei.</param>
    /// <returns>Eine Liste von Objekten des Typs T.</returns>
    public static Task<List<T>> ImportXmlAsync(string filePath)
    {
        return Task.Run(() =>
        {
            var itemList = new List<T>();
            var doc = new XmlDocument();
            doc.Load(filePath);

            var rows = doc.DocumentElement?.SelectNodes("row");
            foreach (XmlNode row in rows!)
            {
                var item = new T();
                foreach (XmlNode childNode in row.ChildNodes)
                {
                    var property = typeof(T).GetProperty(childNode.Name);
                    if (property != null && property.CanWrite)
                        property.SetValue(item, Convert.ChangeType(childNode.InnerText, property.PropertyType));
                }

                itemList.Add(item);
            }

            return itemList;
        });
    }

    /// <summary>
    ///     Exportiert eine Liste von Objekten des Typs T in eine XML-Datei.
    /// </summary>
    /// <param name="items">Die zu exportierende Liste von Objekten.</param>
    /// <param name="filePath">Der Pfad, unter dem die XML-Datei gespeichert wird.</param>
    public static Task ExportXmlAsync(List<T>? items, string filePath)
    {
        var serializer = new XmlSerializer(typeof(List<T>));
        using (TextWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, items);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Importiert eine Liste von Objekten des Typs T aus einer JSON-Datei.
    /// </summary>
    /// <param name="filePath">Der Pfad zur JSON-Datei.</param>
    /// <returns>Eine Liste von Objekten des Typs T oder null, falls der Import fehlschlägt.</returns>
    public static async Task<List<T>?> ImportJsonAsync(string filePath)
    {
        var jsonString = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<T>>(jsonString);
    }

    /// <summary>
    ///     Exportiert eine Liste von Objekten des Typs T in eine JSON-Datei.
    /// </summary>
    /// <param name="items">Die zu exportierende Liste von Objekten.</param>
    /// <param name="filePath">Der Pfad, unter dem die JSON-Datei gespeichert wird.</param>
    public static async Task ExportJsonAsync(List<T> items, string filePath)
    {
        var options = new JsonSerializerOptions {WriteIndented = true};
        var jsonString = JsonSerializer.Serialize(items, options);
        await File.WriteAllTextAsync(filePath, jsonString);
    }
}