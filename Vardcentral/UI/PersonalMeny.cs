
using Vardcentral.Application.Interfaces;
using Vardcentral.Application.Services;
using Vardcentral.Domain;
using Vardcentral.Domain.Enums;


namespace Vardcentral.UI;

public class PersonalMeny
{

    private readonly IBokningsService _tjänst;
    private readonly IPatientRepository _patienter;
    private readonly Besöktyp _akutTyp;
    private readonly AkutBedömare  _bedömare;


    public PersonalMeny(IBokningsService tjänst, IPatientRepository patienter, Besöktyp akutTyp, AkutBedömare  bedömare)
    {
        _tjänst = tjänst;
        _patienter = patienter;
        _akutTyp = akutTyp;
        _bedömare = bedömare;

    }
    public void Kör(Vårdcentral vårdcentral)
    {
        bool klar = false;
        while (!klar)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn + " : bokningspersonal");
            Console.WriteLine("  1. Visa alla aktiva bokningar (mest brådskande först)");
            Console.WriteLine("  2. Akut bedömning ");
            Console.WriteLine("  3. Ställa in ett besök)");
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
        
        List<bool> rödaSvar = StällFrågor(_bedömare.RödaFrågor);
        List<bool> gulaSvar = rödaSvar.Contains(true) ? new List<bool>() : StällFrågor(_bedömare.GulaFrågor);
        BokningsPrioritet färg = _bedömare.Bedöm(rödaSvar, gulaSvar);

        if (färg == BokningsPrioritet.Röd)
        {
            Console.WriteLine("\n RÖD: Ring 112. Ambulansen skickas direkt. Ingen tid bokas i systemet.");
            return;
        }
         if (färg == BokningsPrioritet.Grön)
        {
            Console.WriteLine("\n GRÖN: Inte akut. Patienten bokar en vanlig tid i patientmenyn.");
            return;
        }

        Console.WriteLine("\n GUL: Patienten ska ha en akut tid i dag.");
        Patient? patient = HittaEllerRegistreraPatient();   // akut patient registreras direkt
        if (patient == null) return;

        Bokning? bokning = _tjänst.BokaAkut(_akutTyp, vårdcentral, DateTime.Today, patient);
        if (bokning == null)
        {
            Console.WriteLine("\n Inga akuta tider kvar i dag på någon vårdcentral (öppet vardagar 08-17). " +
                              "Hänvisa till 1177 eller akutmottagningen.");
            return;
        }

        if (bokning.Personal.Vårdcentral.Id != vårdcentral.Id)
        {
            Console.WriteLine($"\n Ingen tid på {vårdcentral.Namn}. Patienten skickas till {bokning.Personal.Vårdcentral.Namn}.");
        }
        Console.WriteLine("\n Gul tid bokad: " + KonsolHjälp.BokningsText(bokning, true));
    }
    private List<bool> StällFrågor(List<string> frågor)
    {
        List<bool> svar = new List<bool>();
        foreach(string fråga in frågor)
        {
            Console.WriteLine("\n " + fråga);
            Console.WriteLine(" 1. Ja");
            Console.WriteLine(" 2. Nej");

            bool ja = KonsolHjälp.LäsHeltal("Val: ", 1, 2) ==1;
            svar.Add(ja);
            if (ja) break;
        }
        return svar;
    }
    private Patient? HittaEllerRegistreraPatient()
    {
        string? personnummer = KonsolHjälp.LäsPersonnummer("Patientens personnummer (10 siffror, Enter = avbryt): ");
        if (personnummer == null) return null;
        
        Patient? patient = _patienter.Hämta(personnummer);
        if (patient != null) return patient;

        Console.WriteLine("Patienten finns inte, registrerad nu: ");
        string förnamn = KonsolHjälp.LäsNamn("Förnamn: ");
        string efternamn = KonsolHjälp.LäsNamn("Efternamn: ");
        patient = new Patient(förnamn, efternamn, personnummer);
        _patienter.LäggTill(patient);
        return patient;
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


    
   



