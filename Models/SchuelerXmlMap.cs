using System.Xml.Serialization;

namespace Models;
[XmlRoot("root")]
public class SchuelerXmlMap
{
    [XmlElement("Klasse")]
    public string Klasse { get; set; }
    
    [XmlElement("Nachname")]
    public string Nachname { get; set; }
    
    [XmlElement("Vorname")]
    public string Vorname { get; set; }
}