
namespace Models;
public class SchuelerXmlMap
{

    public string Klasse { get; set; }
    public string Nachname { get; set; }
    public string Vorname { get; set; }

    public override string ToString()
    {
        return $"Klasse: {Klasse}, Nachname: {Nachname}, Vorname: {Vorname}";
    }
}