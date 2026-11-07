using Vardcentral.Domain;

namespace Vardcentral.UI;

public class KonsolHjälp
{
    public static void Rubrik(string text)
    {
        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine( " " + text);
        Console.WriteLine("=================================");
    }

    public static int LäsHeltal(string fråga, int min, int max)
    {
        while (true)
        {
            Console.Write(fråga);
            string? text = Console.ReadLine();

            if (int.TryParse(text, out int tal)&& tal >= min && tal <= max)
            {
                return tal;
            }

            Console.WriteLine($"Skriv ett tal mellan {min} och {max}.");
        }
    }

    public static string LäsNamn(string fråga)
    {
        while (true)
        {
            Console.Write(fråga);
            string text = (Console.ReadLine()?? "").Trim();

            if (text.Length >=2 && text.All(char.IsLetter))
            {
                return text;
            }

            Console.WriteLine("Skriv minst två bokstäver, utan sifror och mellanslag.");
        }
    }

    public static string? LäsPersonnummer(string fråga)
    {
        while (true)
        {
            Console.Write(fråga);
            string text = (Console.ReadLine()?? "").Trim();

            if (text=="")
            {
                return null;
            }

            if(text.Length== 10 && text.All(char.IsDigit))
            {
                return text;
    
            }

            Console.WriteLine("Personnummer ska vara 10 sifror, till exempel 0001011234");

        }
    }

    public static DateTime LäsDatum(string fråga)
    {
        while (true)
        {
            Console.Write(fråga);
            string text = (Console.ReadLine()?? "").Trim();

            if (text == "")
            {
                return DateTime.Today;
            }

            if (DateTime.TryParse(text, out DateTime datum) && datum.Date >= DateTime.Today )
            {
                return datum.Date;
            }

            Console.WriteLine("Skriv ett datum i formatet åååå-mm-dd (Idag eller senare), eller tryck Enter." );
        }
    }


    public static string BokningsText(Bokning b, bool visaPatient)
    {
        string text = $"#{b.Id} {b.Tid.Start:yyyy-MM-dd HH:mm} {b.Typ.Namn}" +
        $" {b.Personal.Namn} {b.Personal.Efternamn}, {b.Personal.Vårdcentral.Namn}," +
        $"{b.Rum.Namn} [{b.Status}, {b.Prioritet}]";

        if (visaPatient)
        {
            text += $" Patient: {b.Patient.Namn} {b.Patient.Efternamn}";
        }
        return text;
    }

    public static Bokning? VäljBokning(List<Bokning>bokningar, bool visaPatient)
    {
        if (bokningar.Count == 0)
        {
            Console.WriteLine("Det finns inga bokningar att välja bland.");
            return null;
        }

        Console.WriteLine();

        for(int i = 0; i <bokningar.Count; i++)
        {
            Console.WriteLine($" {i+1}. {BokningsText(bokningar[i], visaPatient)}");
        }
        Console.WriteLine("0. Avbryt");

        int val = LäsHeltal("\nVal: ",0,bokningar.Count);

        if(val == 0)
        {
            return null;
        }
        return bokningar[val-1];
    }

    public static Tidslucka? VäljTid(List<Tidslucka> tider)
    {
        List<Tidslucka> visade = tider.Where(t => t.Start.Minute %15 == 0).Take (12).ToList();
        if(visade.Count == 0)
        {
            visade = tider.Take(12).ToList();
        }
        
        Console.WriteLine("\n Ledig tider :");
        
        for (int i = 0; i < visade.Count; i++)
        {
            Console.WriteLine($" {i + 1}. {visade[i].Start:HH:mm}");
                    
        }
        Console.WriteLine("0. Avbryt");
        
        int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, visade.Count);
                    
        if (val == 0) return null;
        
        return visade[val - 1];
    }

    public static void SkrivBokningar(List<Bokning> bokningar, bool visaPatient, string tomText)
    {
        Console.WriteLine();
        if (bokningar.Count == 0)
        {
            Console.WriteLine(" " + tomText);
            return;
        }

        foreach (Bokning bokning in bokningar)
        {
            Console.WriteLine("" + BokningsText(bokning,visaPatient));
        }
        
    }


}