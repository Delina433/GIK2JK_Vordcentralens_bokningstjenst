
using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;
namespace Vardcentral.UI;

public class PersonalMeny
{

    private readonly IBokningsService _tjänst;
    private readonly IPatientRepository _patienter;
    private readonly Besöktyp _akutTyp;


    public PersonalMeny(IBokningsService tjänst, IPatientRepository patienter, Besöktyp akutTyp)
    {
        _tjänst = tjänst;
        _patienter = patienter;
        _akutTyp = akutTyp;

    }
    public void Kör(Vårdcentral vårdcentral)
    {
        bool klar = false;
        while (!klar)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn + " : bokningspersonal");
            Console.WriteLine("  1. Visa alla aktiva bokningar (mest brådskande först)");
            Console.WriteLine("  2. Akut bedömning (röd/gul/grön)");
            Console.WriteLine("  3. Ställa in ett besök(t.ex. personal sjuk)");
            Console.WriteLine("  4. Avboka en patients bokning");
            Console.WriteLine("  0. Tillbaka");

            int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, 4);

            switch (val)
            {

                case 1:
                    VisaAktiva(vårdcentral);
                    break;
                case 2:
                    AkutBedömning(vårdcentral);
                    break;
                case 3:
                    StällIn(vårdcentral);
                    break;
                case 4:
                    Avboka(vårdcentral);
                    break;
                case 0:
                    klar = true;
                    break;
            }

        }

    }

    private List<Bokning> AktivaHär(Vårdcentral vårdcentral)
    {
        List<Bokning> resultat = new List<Bokning>();
        foreach (var bokning in _tjänst.HämtaAktivaSorterade())
        {


            if (bokning.Personal.Vårdcentral.Id == vårdcentral.Id)
            {
                resultat.Add(bokning);

            }
        }
        return resultat;
        }
    

    private void VisaAktiva(Vårdcentral vårdcentral)
    {
        List<Bokning> aktiva = AktivaHär(vårdcentral);
        
            Console.WriteLine();
            if (aktiva.Count == 0)
            {
                Console.WriteLine("  Ingen aktiva bokningar.");
                return;
            }

            foreach (Bokning bokning in aktiva)
            {
                Console.WriteLine("  " + KonsolHjälp.BokningsText(bokning, true));
            }
    }
    private void AkutBedömning(Vårdcentral vårdcentral)
    {
        KonsolHjälp.Rubrik("AKUT BEDÖMNING");
        Console.WriteLine(" 1. Röd - Livshotande, ambulans direkt");
        Console.WriteLine(" 2. Gul - Akut, tidigaste tid i dag");
        Console.WriteLine(" 3. Grön - Inte akut, vanlig bokning");
        Console.WriteLine(" 0. Avbryt");

        int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, 3);

        if (val == 0)
            return;
        if (val == 1)
        {
            Console.WriteLine("\n Röd: Ring 112. Ambulans skickas direkt. Ingen tid bokas i systemet.");
            return;
        }
        if (val == 3)
        {
            Console.WriteLine("\n Grön: Patienten får de tider som är kvar. Patienten bokas in som vanligt.");
            return;
        }
        string? personnummer = KonsolHjälp.LäsPersonnummer("Patientens personnummer (10 siffror): ");
        if (personnummer == null)
        {
            Console.WriteLine("\n Ogiltigt personnummer.");
            return;
        }


        Patient? patient = _patienter.Hämta(personnummer);
        if (patient == null)
        {
            Console.WriteLine("\n Patient är inte registrerad.Patienten behöver först boka en tid i patientmenyn.");

            return;
        }

        Bokning? bokning = _tjänst.BokaAkut(_akutTyp, vårdcentral, DateTime.Today, patient);
        if (bokning == null)
        {
            Console.WriteLine("\n Ingen ledig akuttid idag på någon vårdcentral. Överväg röd(ambulans).");

            return;
        }

        if (bokning.Personal.Vårdcentral.Id != vårdcentral.Id)
        {
            Console.WriteLine($"\n  Ingen tid på {vårdcentral.Namn}. Patienten skickas till {bokning.Personal.Vårdcentral.Namn}.");
        }
        Console.WriteLine($"\n  Gul Tid bokad : " + KonsolHjälp.BokningsText(bokning, true));

    }
    private void StällIn(Vårdcentral vårdcentral)
    {
        Console.WriteLine("\n Vilket besök ska ställas in?");
        Bokning? bokning = KonsolHjälp.VäljBokning(AktivaHär(vårdcentral), true);

        if (bokning == null)
            return;

        if (!_tjänst.StällIn(bokning.Id))
            {
                Console.WriteLine(" Bokningen gick inte att ställa in.");

            }
    }

    private void Avboka(Vårdcentral vårdcentral)
    {
        Console.WriteLine("\n Vilken bokning ska avbokas?");
       Bokning? bokning = KonsolHjälp.VäljBokning(AktivaHär(vårdcentral), true);
        if (bokning == null)
            return;

        if (!_tjänst.Avboka(bokning.Id))
        {
            Console.WriteLine(" Bokningen gick inte att avboka.");

        }
    }
        
}


    
   



