using Vardcentral.Domain;

namespace Vardcentral.UI;

/// Innehåller gemensamma hjälpmetoder för konsolgränssnittet.
/// Hanterar bland annat inmatning, validering och utskrift av bokningar.

public class KonsolHjälp
{

    /// Skriver ut en rubrik med linjer ovanför och under texten.
    /// Texten som ska visas som rubrik.
    public static void Rubrik(string text)
    {
        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine(" " + text);
        Console.WriteLine("=================================");
    }

    
    /// Läser in ett heltal från användaren och kontrollerar att
    /// det ligger inom det angivna intervallet.
    /// Fortsätter fråga tills användaren anger ett giltigt tal.
   
    public static int LäsHeltal(string fråga, int min, int max)
    {
        while (true)
        {
            Console.Write(fråga);
            string? text = Console.ReadLine();

            // Kontrollerar att inmatningen går att omvandla till ett heltal
            // och att värdet ligger inom det tillåtna intervallet.
            if (int.TryParse(text, out int tal) && tal >= min && tal <= max)
            {
                return tal;
            }

            Console.WriteLine($"Skriv ett tal mellan {min} och {max}.");
        }
    }

    
    /// Läser in ett namn och kontrollerar att det innehåller minst två bokstäver och inga siffror eller mellanslag.
    public static string LäsNamn(string fråga)
    {
        while (true)
        {
            Console.Write(fråga);
            string text = (Console.ReadLine() ?? "").Trim();

            // Kontrollerar att namnet innehåller minst två tecken och att samtliga tecken är bokstäver.
           
            if (text.Length >= 2 && text.All(char.IsLetter))
            {
                return text;
            }

            Console.WriteLine("Skriv minst två bokstäver, utan siffror och mellanslag.");
        }
    }

    
    /// Läser in ett personnummer och kontrollerar att det innehåller
    /// exakt tio siffror. Användaren kan även lämna fältet tomt.
    public static string? LäsPersonnummer(string fråga)
    {
        while (true)
        {
            Console.Write(fråga);
            string text = (Console.ReadLine() ?? "").Trim();

            // Om användaren trycker Enter utan att skriva något returneras null.
            if (text == "")
            {
                return null;
            }

            // Kontrollerar att personnumret består av exakt tio siffror.
            if (text.Length == 10 && text.All(char.IsDigit))
            {
                return text;
            }

            Console.WriteLine("Personnummer ska vara 10 siffror, till exempel 0001011234");
        }
    }

    
    /* Läser in ett datum och kontrollerar att det är dagens datum
     eller ett datum i framtiden. Om användaren trycker Enter
     används dagens datum automatiskt.*/
    public static DateTime LäsDatum(string fråga)
    {
        while (true)
        {
            Console.Write(fråga);
            string text = (Console.ReadLine() ?? "").Trim();

            // Om användaren inte anger något datum används dagens datum.
            if (text == "")
            {
                return DateTime.Today;
            }

            /* Kontrollerar att texten kan omvandlas till ett datum
             och att datumet inte ligger före dagens datum.*/
            if (DateTime.TryParse(text, out DateTime datum) && datum.Date >= DateTime.Today)
            {
                return datum.Date;
            }

            Console.WriteLine("Skriv ett datum i formatet åååå-mm-dd (Idag eller senare), eller tryck Enter.");
        }
    }

    /// Skapar en sammanhängande text med information om en bokning.
    /// Patienten visas endast om parametern visaPatient är true.
    public static string BokningsText(Bokning b, bool visaPatient)
    {
        // Samlar bokningens information i en text.
        string text = $"#{b.Id} {b.Tid.Start:yyyy-MM-dd HH:mm} {b.Typ.Namn}" +
        $" {b.Personal.Namn} {b.Personal.Efternamn}, {b.Personal.Vårdcentral.Namn}," +
        $"{b.Rum.Namn} [{b.Status}, {b.Prioritet}]";

        // Lägger till patientens namn om det ska visas.
        if (visaPatient)
        {
            text += $" Patient: {b.Patient.Namn} {b.Patient.Efternamn}";
        }

        return text;
    }

    /// Visar en lista med bokningar och låter användaren välja en.
    /// Användaren kan även avbryta valet.
    public static Bokning? VäljBokning(List<Bokning> bokningar, bool visaPatient)
    {
        // Kontrollerar om det finns några bokningar att välja bland.
        if (bokningar.Count == 0)
        {
            Console.WriteLine("Det finns inga bokningar att välja bland.");
            return null;
        }

        Console.WriteLine();

        // Skriver ut alla bokningar med varsitt nummer.
        for (int i = 0; i < bokningar.Count; i++)
        {
            Console.WriteLine($" {i + 1}. {BokningsText(bokningar[i], visaPatient)}");
        }

        Console.WriteLine("0. Avbryt");

        // Läser in användarens val och kontrollerar att det är giltigt.
        int val = LäsHeltal("\nVal: ", 0, bokningar.Count);

        // Returnerar null om användaren väljer att avbryta.
        if (val == 0)
        {
            return null;
        }

        // Returnerar den bokning som användaren har valt.
        return bokningar[val - 1];
    }

    /// Visar tillgängliga tidsluckor och låter användaren välja en.
    /// Visar i första hand tider som börjar på en kvart (00, 15, 30 eller 45).
    public static Tidslucka? VäljTid(List<Tidslucka> tider)
    {
        // Filtrerar fram tider som börjar på en kvart och begränsar listan till 12 alternativ.
        List<Tidslucka> visade = tider.Where(t => t.Start.Minute % 15 == 0).Take(12).ToList();

        // Om inga tider uppfyller villkoret visas istället de första 12 tillgängliga tiderna.
        if (visade.Count == 0)
        {
            visade = tider.Take(12).ToList();
        }

        Console.WriteLine("\n Lediga tider:");

        // Skriver ut de valbara tiderna med varsitt nummer.
        for (int i = 0; i < visade.Count; i++)
        {
            Console.WriteLine($" {i + 1}. {visade[i].Start:HH:mm}");
        }

        Console.WriteLine("0. Avbryt");

        // Läser in användarens val.
        int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, visade.Count);

        // Returnerar null om användaren väljer att avbryta.
        if (val == 0) return null;

        // Returnerar den valda tidsluckan.
        return visade[val - 1];
    }

    
    /// Skriver ut en lista med bokningar.
    /// Om listan är tom visas ett anpassat meddelande.
    public static void SkrivBokningar(List<Bokning> bokningar, bool visaPatient, string tomText)
    {
        Console.WriteLine();

        // Kontrollerar om listan saknar bokningar.
        if (bokningar.Count == 0)
        {
            Console.WriteLine(" " + tomText);
            return;
        }

        // Går igenom alla bokningar och skriver ut informationen om varje bokning.
        foreach (Bokning bokning in bokningar)
        {
            Console.WriteLine("" + BokningsText(bokning, visaPatient));
        }
    }
}
