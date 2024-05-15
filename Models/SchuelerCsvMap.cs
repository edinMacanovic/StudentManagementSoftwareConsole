using CsvHelper.Configuration;
using Models;

namespace DataManager;

public sealed class SchuelerCsvMap : ClassMap<Schueler>
{
    public SchuelerCsvMap()
    {
        Map(m => m.Klasse).Name("Klasse");
        Map(m => m.Nachname).Name("Nachname");
        Map(m => m.Vorname).Name("Vorname");
    }
}