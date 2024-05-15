using Models;

namespace DataManager;

/// <summary>
/// Stellt Funktionen zur Generierung von E-Mail-Adressen für Schüler bereit.
/// </summary>
public class EmailGenerator
{
    /// <summary>
    /// Generiert eindeutige E-Mail-Adressen für eine Liste von Schülern.
    /// </summary>
    /// <param name="schuelerListe">Eine Liste von Schülerobjekten, für die E-Mails generiert werden sollen.</param>
    public static void GenerateEmails(List<Schueler> schuelerListe)
    {
        // Wörterbuch zur Nachverfolgung der Anzahl von E-Mails basierend auf dem E-Mail-Basisnamen
        var emailCounts = new Dictionary<string, int>();

        foreach (var schueler in schuelerListe)
        {
            // Basis-E-Mail-Adresse erstellen: Vorname.Nachname, umgewandelt in Kleinbuchstaben und ohne Leerzeichen
            var baseEmail = $"{schueler.Vorname}.{schueler.Nachname}".ToLower().Replace(" ", "");
            // Entfernen ungültiger Zeichen aus der E-Mail
            var validEmail = RemoveInvalidChars(baseEmail);
            int count;

            // Prüfen, ob die E-Mail bereits vergeben wurde und gegebenenfalls die Anzahl der Duplikate erhöhen
            if (emailCounts.TryGetValue(validEmail, out count))
            {
                count++;
                schueler.Email = $"{validEmail}{count}@schule.de";  // Duplikat vorhanden, Zahl anhängen
                emailCounts[validEmail] = count;
            }
            else
            {
                schueler.Email = $"{validEmail}@schule.de";  // Kein Duplikat, Standardformat verwenden
                emailCounts.Add(validEmail, 1);
            }
        }
    }

    /// <summary>
    /// Entfernt ungültige Zeichen aus einer E-Mail-Adresse, um die Gültigkeit sicherzustellen.
    /// </summary>
    /// <param name="input">Die rohe Eingabezeichenkette, die auf Gültigkeit geprüft wird.</param>
    /// <returns>Eine bereinigte Zeichenkette, die nur Buchstaben, Ziffern und Punkte enthält.</returns>
    private static string RemoveInvalidChars(string input)
    {
        // Filtern der Eingabe, erlaubt sind nur Buchstaben, Ziffern und Punkte
        return new string(input.Where(c => char.IsLetterOrDigit(c) || c == '.').ToArray());
    }
}