using Models;

public class EmailGenerator
{
    public static void GenerateEmails(List<Schueler> schuelerListe)
    {
        var emailCounts = new Dictionary<string, int>();

        foreach (var schueler in schuelerListe)
        {
            var baseEmail = $"{schueler.Vorname}.{schueler.Nachname}".ToLower().Replace(" ", "");
            var validEmail = RemoveInvalidChars(baseEmail);
            int count;

            if (emailCounts.TryGetValue(validEmail, out count))
            {
                count++;
                schueler.Email = $"{validEmail}{count}@schule.de";
                emailCounts[validEmail] = count;
            }
            else
            {
                schueler.Email = $"{validEmail}@schule.de";
                emailCounts.Add(validEmail, 1);
            }
        }
    }

    private static string RemoveInvalidChars(string input)
    {
        return new string(input.Where(c => char.IsLetterOrDigit(c) || c == '.').ToArray());
    }
}