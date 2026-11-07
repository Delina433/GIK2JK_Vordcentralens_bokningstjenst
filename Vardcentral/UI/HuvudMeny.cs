using Vardcentral.Domain;

namespace Vardcentral.UI;

public class HuvudMeny
{

    private readonly List<Vårdcentral> _vårdcentraler;
    private readonly PatientMeny _patientMeny;
    private readonly PersonalMeny _personalMeny;

    public HuvudMeny(List<Vårdcentral> vårdcentraler, PatientMeny patientMeny, PersonalMeny personalMeny)
    {
        _vårdcentraler = vårdcentraler;
        _patientMeny = patientMeny;
        _personalMeny = personalMeny;
    }
    public void Kör()
    {
        while (true)
        {
            KonsolHjälp.Rubrik("VÅRDCENTRALENS BOKNINGSTJÄNST");
            for (int i = 0; i < _vårdcentraler.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_vårdcentraler[i].Namn}");
            }
                Console.WriteLine("  0. Avsluta");

                int nr = KonsolHjälp.LäsHeltal("Val:  : ", 0, _vårdcentraler.Count);
                if (nr == 0)
                {
                    Console.WriteLine("Avslutar programmet.");
                    return;
                }
                VäljRoll(_vårdcentraler[nr - 1]);
            }
        }
    
            
        
    
            private void VäljRoll(Vårdcentral vårdcentral)
    {
        while (true)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn + " :vem är du?");
            Console.WriteLine("  1. Jag är en patient");
            Console.WriteLine("  2. Jag är personal");
            Console.WriteLine("  0. Byt vårdcentral");

            int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, 2);

            switch (val)
            {
                case 1:
                    _patientMeny.Kör(vårdcentral);
                    break;
                case 2:
                    _personalMeny.Kör(vårdcentral);
                    break;
                case 0:
                    return;
            }
        }
    }
}
