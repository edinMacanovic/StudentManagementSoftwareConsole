using System.Diagnostics;
using DataManager;
using Models;

namespace StudentManagementSoftwareConsole;

public class Program
{
    private static async Task Main(string[] args)
    {
        var schuelerListe = new List<Schueler>();
        var exit = false;

        while (!exit)
        {
            Console.WriteLine("\nWählen Sie eine Option:");
            Console.WriteLine("1: CSV-Datei importieren und E-Mails generieren");
            Console.WriteLine("2: Liste als XML-Datei exportieren");
            Console.WriteLine("3: Liste als JSON-Datei exportieren");
            Console.WriteLine("4: Statistik anzeigen");
            Console.WriteLine("5: Programm beenden");
            Console.Write("Ihre Wahl: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Dateipfad für den CSV-Import eingeben: ");
                    schuelerListe = await DataManager<Schueler>.ImportCsvAsync(await ChooseFileAsync("csv"), new SchuelerCsvMap());
                    EmailGenerator.GenerateEmails(schuelerListe);

                    Console.WriteLine("CSV-Import und E-Mail-Generierung erfolgreich!");
                    Console.WriteLine("Aktualisierte Schülerliste:");
                    foreach (var schueler in schuelerListe)
                        Console.WriteLine(
                            $"Klasse: {schueler.Klasse}, Vorname: {schueler.Vorname}, Nachname: {schueler.Nachname}, E-Mail: {schueler.Email}");
                    break;
                case "2":
                    Console.Write("Wählen Sie den XML-Dateipfad aus: ");
                    var xmlPath = await ChooseFileAsync("xml");
                    var xmlSchuelerListe = await DataManager<SchuelerXmlMap>.ImportXmlAsync(xmlPath);
                    if (!string.IsNullOrEmpty(xmlPath))
                    {
                        Console.WriteLine("Möchten Sie diese XML-Datei wirklich speichern? (ja/nein)");
                        if (Console.ReadLine().Trim().ToLower() == "ja")
                        {
                            var newXmlPath = GenerateNewFilePath(xmlPath);
                            await DataManager<SchuelerXmlMap>.ExportXmlAsync(xmlSchuelerListe, newXmlPath);
                            Console.WriteLine($"XML-Export erfolgreich! Gespeichert als {newXmlPath}");

                            // Datei öffnen
                            Process.Start(new ProcessStartInfo(newXmlPath) { UseShellExecute = true });
                        }
                        else
                        {
                            Console.WriteLine("XML-Export abgebrochen.");
                        }
                    }
                    break;
                case "3":
                    Console.Write("Wählen Sie den JSON-Dateipfad aus: ");
                    var jsonPath = await ChooseFileAsync("json");
                    var jsonSchuelerListe = await DataManager<SchuelerJsonMap>.ImportJsonAsync(jsonPath);
                    if (!string.IsNullOrEmpty(jsonPath))
                    {
                        Console.WriteLine("Möchten Sie diese JSON-Datei wirklich speichern? (ja/nein)");
                        if (Console.ReadLine()?.Trim().ToLower() == "ja")
                        {
                            var newJsonPath = GenerateNewFilePath(jsonPath);
                            await DataManager<SchuelerJsonMap>.ExportJsonAsync(jsonSchuelerListe!, newJsonPath);
                            Console.WriteLine($"JSON-Export erfolgreich! Gespeichert als {newJsonPath}");

                            // Datei öffnen
                            Process.Start(new ProcessStartInfo(newJsonPath) { UseShellExecute = true });
                        }
                        else
                        {
                            Console.WriteLine("JSON-Export abgebrochen.");
                        }
                    }
                    break;
                case "4":
                    if (schuelerListe.Any()) // Prüfen, ob die Liste Elemente enthält
                    {
                        // Anzahl der Schüler pro Klasse
                        var schuelerProKlasse = schuelerListe
                            .GroupBy(x => x.Klasse)
                            .Select(gruppe => new { Klasse = gruppe.Key, Anzahl = gruppe.Count() });

                        // Gesamtanzahl der Klassen
                        var klassenAnzahl = schuelerProKlasse.Count();

                        // Gesamtanzahl der Schüler
                        var gesamtSchueler = schuelerListe.Count;

                        // Durchschnittliche Anzahl der Schüler pro Klasse
                        var durchschnittSchuelerProKlasse = schuelerListe
                            .GroupBy(s => s.Klasse)
                            .Average(gruppe => gruppe.Count());

                        Console.WriteLine("Statistik der Schüler und Klassen:");
                        Console.WriteLine($"Gesamtanzahl der Klassen: {klassenAnzahl}");
                        Console.WriteLine($"Gesamtanzahl der Schüler: {gesamtSchueler}");
                        Console.WriteLine($"Durchschnittliche Schülerzahl pro Klasse: {durchschnittSchuelerProKlasse:F2}");

                        foreach (var klasse in schuelerProKlasse)
                        {
                            Console.WriteLine($"Klasse {klasse.Klasse} hat {klasse.Anzahl} Schüler.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Keine Schülerdaten verfügbar.");
                    }
                    break;
                case "5":
                    exit = true;
                    Console.WriteLine("Programm beendet.");
                    break;
                default:
                    Console.WriteLine("Ungültige Auswahl, bitte erneut versuchen.");
                    break;
            }
        }
    }

    private static Task<string> ChooseFileAsync(string fileType)
    {
        Console.WriteLine($"Verfügbare {fileType.ToUpper()} Dateien im Verzeichnis 'Data':");
        var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FileData");
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
            Console.WriteLine(
                $"Keine Dateien gefunden. Stellen Sie sicher, dass {fileType.ToUpper()} Dateien im 'Data'-Verzeichnis liegen.");
            return Task.FromResult<string>(null);
        }

        var files = Directory.EnumerateFiles(dataDirectory, $"*.{fileType}").ToList();
        if (files.Count == 0)
        {
            Console.WriteLine($"Keine {fileType.ToUpper()} Dateien im Verzeichnis gefunden.");
            return Task.FromResult<string>(null);
        }

        for (var i = 0; i < files.Count; i++) Console.WriteLine($"{i + 1}: {Path.GetFileName(files[i])}");

        Console.Write("Wählen Sie eine Datei durch Eingabe der Nummer: ");
        if (int.TryParse(Console.ReadLine(), out var fileIndex) && fileIndex > 0 && fileIndex <= files.Count)
            return Task.FromResult(files[fileIndex - 1]);

        return Task.FromResult<string>(null);
    }
    
    private static string GenerateNewFilePath(string originalPath)
    {
        var directory = Path.GetDirectoryName(originalPath);
        var fileName = Path.GetFileNameWithoutExtension(originalPath);
        var extension = Path.GetExtension(originalPath);
        var dateTime = DateTime.Now.ToString("yyyyMMdd");
        return Path.Combine(directory, $"{fileName}_{dateTime}{extension}");
    }

}