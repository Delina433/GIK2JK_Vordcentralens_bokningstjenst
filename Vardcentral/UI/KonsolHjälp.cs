namespace Vardcentral.UI;

public class KonsolHjälp
{
    public static void Rubrik(string text)
    {
        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine( " ") +text);
        Console.WriteLine("=================================");
    }

    public static int LäsHeltal(string fråga, int min, int max)
    {
        While (true)
        {
            Console.WriteLine(fråga);
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
                return null;
            }

            if (text.Length == 10 && text.All(char.IsDigit))
            {
                return text;
            }

            Console.WriteLine("Personnummer ska vara 10 siffror, till exempel 0001011234.");
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

            if (DateTime.TryParse(text, out DateTime datum) && datum.Date  >= DateTime.Today)
            {
                return datum.Date;
            }

            Console.WriteLine("Skriv ett datum i formatet åååå-mm-dd (Idag eller senare), eller tryck Enter.");
        }
    }
    public static string BokningsText(Bokning b, bool visaPatient)
    {
        string text = $"#{b.Id} {b.Tid.Start:yyyy-MM-dd HH:mm} {b.Typ.Namn}" +
        $" {b.Personal.Namn} {b.Person.Efternamn}, {b.Personal.Vårdcentral.Namn}" +
        $"{b.Rum.Nman} [{b.Status}, {b.Prioritet}]";

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

        if(val = 0)
        {
            return null;
        }
        return bokningar[val-1];
    }

}