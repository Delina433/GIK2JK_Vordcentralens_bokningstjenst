using Vardcentral.Domain;

namespace Vardcentral.UI;

/// Huvudmenyn i bokningssystemet.
/// Låter användaren välja vårdcentral
/// Låter användaren välja om användaren vill fortsätta som patient eller personal.

public class HuvudMeny
{
    // Lista med alla tillgängliga vårdcentraler.
    private readonly List<Vårdcentral> _vårdcentraler;

    // Hanterar patientmenyn.
    private readonly PatientMeny _patientMeny;

    // Hanterar personalmenyn.
    private readonly PersonalMeny _personalMeny;

  
    /// Skapar huvudmenyn och tar emot vårdcentraler,
    /// patientmenyn och personalmenyn.
  
    public HuvudMeny(List<Vårdcentral> vårdcentraler, PatientMeny patientMeny, PersonalMeny personalMeny)
    {
        _vårdcentraler = vårdcentraler;
        _patientMeny = patientMeny;
        _personalMeny = personalMeny;
    }

    
    /// Startar huvudmenyn där användaren får välja vårdcentral.
    /// Menyn fortsätter visas tills användaren väljer att avsluta programmet.
    
    public void Kör()
    {
        // Loopen håller huvudmenyn igång tills användaren väljer att avsluta.
        while (true)
        {
            KonsolHjälp.Rubrik("VÅRDCENTRALENS BOKNINGSTJÄNST");
            Console.WriteLine("Välj vårdcentral: ");

            // Skriver ut alla tillgängliga vårdcentraler med varsitt nummer.
            for (int i = 0; i < _vårdcentraler.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_vårdcentraler[i].Namn}");
            }

            Console.WriteLine(" 0. Avsluta");

            // Läser in användarens val och kontrollerar att det ligger inom tillåtet intervall.
            int nr = KonsolHjälp.LäsHeltal(" Val: : ", 0, _vårdcentraler.Count);

            // Avslutar programmet om användaren väljer 0.
            if (nr == 0)
            {
                Console.WriteLine("Avslutar programmet.");
                return;
            }

            // Skickar den valda vårdcentralen vidare till menyn för rollval.
            VäljRoll(_vårdcentraler[nr - 1]);
        }
    }

    
    /// Låter användaren välja om den vill fortsätta som patient eller personal.

   
    private void VäljRoll(Vårdcentral vårdcentral)
    {
        // Håller rollvalsmenyn igång tills användaren väljer att byta vårdcentral.
        while (true)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn + " :vem är du?");
            Console.WriteLine("  1. Jag är en patient");
            Console.WriteLine("  2. Jag är personal");
            Console.WriteLine("  0. Byt vårdcentral");

            // Läser in och kontrollerar användarens val.
            int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, 2);

            // Skickar användaren vidare till rätt meny beroende på valet.
            switch (val)
            {
                case 1:
                    _patientMeny.Kör(vårdcentral);
                    break;

                case 2:
                    _personalMeny.Kör(vårdcentral);
                    break;

                case 0:
                    // Går tillbaka till huvudmenyn så att användaren kan välja en annan vårdcentral.
                    return;
            }
        }
    }
}
