
using Vardcentral.Domain;

namespace Vardcentral.UI;

/// Hanterar programmets huvudmeny.
/// Låter användaren välja vårdcentral och därefter vilken roll
/// användaren har, patient eller personal.
public class HuvudMeny
{
    /// Lista över alla vårdcentraler som finns i systemet.
    private readonly List<Vårdcentral> _vårdcentraler;

    /// Menyn som hanterar patientens funktioner.
    private readonly PatientMeny _patientMeny;

    /// Menyn som hanterar personalens funktioner.
    private readonly PersonalMeny _personalMeny;

    /// Skapar huvudmenyn och tar emot de beroenden som behövs.
    public HuvudMeny(List<Vårdcentral> vårdcentraler, PatientMeny patientMeny, PersonalMeny personalMeny)
    {
        /// Sparar de objekt som skickas in via konstruktorn.
        _vårdcentraler = vårdcentraler;
        _patientMeny = patientMeny;
        _personalMeny = personalMeny;
    }

    /// Startar programmets huvudmeny.
    /// Visar tillgängliga vårdcentraler och låter användaren välja en.
    public void Kör()
    {
        /// Loopen fortsätter tills användaren väljer att avsluta programmet.
        while (true)
        {
            KonsolHjälp.Rubrik("VÅRDCENTRALENS BOKNINGSTJÄNST");
            Console.WriteLine("Välj vårdcentral: ");

            /// Går igenom listan och visar alla vårdcentraler.
            for (int i = 0; i < _vårdcentraler.Count; i++)
            {
                /// i + 1 används eftersom användarens val börjar på 1,
                /// medan listans index börjar på 0.
                Console.WriteLine($"{i + 1}. {_vårdcentraler[i].Namn}");
            }

            Console.WriteLine(" 0. Avsluta");

            /// Läser in användarens val.
            /// Värdet måste ligga mellan 0 och antalet vårdcentraler.
            int nr = KonsolHjälp.LäsHeltal(" Val: : ", 0, _vårdcentraler.Count);

            /// Om användaren väljer 0 avslutas metoden och programflödet lämnar huvudmenyn.
            if (nr == 0)
            {
                Console.WriteLine("Avslutar programmet.");
                return;
            }

            /// Hämtar den valda vårdcentralen från listan.
            /// -1 behövs eftersom listans index börjar på 0.
            /// Skickar sedan vårdcentralen vidare till metoden VäljRoll.
            VäljRoll(_vårdcentraler[nr - 1]);
        }
    }

    /// Visar en meny där användaren får välja om hen är patient eller personal.
    /// Anropar därefter rätt meny för den valda rollen.
    private void VäljRoll(Vårdcentral vårdcentral)
    {
        /// Fortsätter visa rollmenyn tills användaren väljer att byta vårdcentral.
        while (true)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn + " :vem är du?");
            Console.WriteLine("  1. Jag är en patient");
            Console.WriteLine("  2. Jag är personal");
            Console.WriteLine("  0. Byt vårdcentral");

            /// Läser in användarens val mellan 0 och 2.
            int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, 2);

            /// Anropar rätt meny beroende på vilken roll användaren väljer.
            switch (val)
            {
                case 1:
                    /// Öppnar patientmenyn och skickar med vald vårdcentral.
                    _patientMeny.Kör(vårdcentral);
                    break;

                case 2:
                    /// Öppnar personalmenyn och skickar med vald vårdcentral.
                    _personalMeny.Kör(vårdcentral);
                    break;

                case 0:
                    /// Avslutar rollmenyn och återgår till valet av vårdcentral.
                    return;
            }
        }
    }
}
