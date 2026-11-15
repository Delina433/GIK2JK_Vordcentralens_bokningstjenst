
using Vardcentral.Application.Interfaces;
using Vardcentral.Application.Services;
using Vardcentral.Domain;
using Vardcentral.Domain.Enums;

namespace Vardcentral.UI;

/// Hanterar bokningspersonalens meny och funktioner.
/// Personalen kan visa aktiva bokningar, göra akuta bedömningar,
/// ställa in besök och avboka patienters bokningar.
public class PersonalMeny
{
    // Används för att kommunicera med bokningstjänsten.
    private readonly IBokningsService _tjänst;

    // Används för att hämta och registrera patienter.
    private readonly IPatientRepository _patienter;

    // Besökstypen som används vid akuta bokningar.
    private readonly Besöktyp _akutTyp;

    // Hanterar bedömningen av patientens akuta tillstånd.
    private readonly AkutBedömare _bedömare;

    /// Skapar personalmenyn och tar emot de tjänster,
    /// repository och objekt som behövs för funktionerna.
    public PersonalMeny(IBokningsService tjänst, IPatientRepository patienter, Besöktyp akutTyp, AkutBedömare bedömare)
    {
        _tjänst = tjänst;
        _patienter = patienter;
        _akutTyp = akutTyp;
        _bedömare = bedömare;
    }

    /// Visar personalens huvudmeny och låter personalen
    /// välja vilken funktion som ska användas.
    
    public void Kör(Vårdcentral vårdcentral)
    {
        bool klar = false;

        // Menyn fortsätter visas tills personalen väljer att gå tillbaka.
        while (!klar)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn + " : Bokningspersonal");
            Console.WriteLine("  1. Visa alla aktiva bokningar (mest brådskande först)");
            Console.WriteLine("  2. Akut bedömning ");
            Console.WriteLine("  3. Ställa in ett besök");
            Console.WriteLine("  4. Avboka en patients bokning");
            Console.WriteLine("  0. Tillbaka");

            // Läser in och kontrollerar personalens menyval.
            int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, 4);

            // Anropar rätt metod beroende på personalens val.
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

    /// Hämtar alla aktiva bokningar och filtrerar fram
    /// de bokningar som tillhör den valda vårdcentralen.
    /// Bokningarna behåller sin prioriteringsordning.
    private List<Bokning> AktivaHär(Vårdcentral vårdcentral)
    {
        List<Bokning> resultat = new List<Bokning>();

        // Hämtar aktiva bokningar som redan är sorterade efter prioritet.
        foreach (var bokning in _tjänst.HämtaAktivaSorterade())
        {
            // Kontrollerar att bokningen tillhör den valda vårdcentralen.
            if (bokning.Personal.Vårdcentral.Id == vårdcentral.Id)
            {
                resultat.Add(bokning);
            }
        }

        return resultat;
    }

    /// Visar alla aktiva bokningar som tillhör vårdcentralen.
    /// Bokningarna visas i prioriteringsordning.
    private void VisaAktiva(Vårdcentral vårdcentral)
    {
        // Hämtar vårdcentralens aktiva bokningar.
        List<Bokning> aktiva = AktivaHär(vårdcentral);

        Console.WriteLine();

        // Kontrollerar om det finns några aktiva bokningar.
        if (aktiva.Count == 0)
        {
            Console.WriteLine("  Ingen aktiva bokningar.");
            return;
        }

        // Skriver ut samtliga bokningar med patientens information.
        foreach (Bokning bokning in aktiva)
        {
            Console.WriteLine("  " + KonsolHjälp.BokningsText(bokning, true));
        }
    }

    /// Genomför en akut bedömning genom att ställa frågor till patienten.
    /// Beroende på svaren avgörs om patienten behöver akut hjälp,
    /// en akut bokning eller en vanlig besökstid.
    private void AkutBedömning(Vårdcentral vårdcentral)
    {
        KonsolHjälp.Rubrik("AKUT BEDÖMNING");

        // Ställer först de röda frågorna för att identifiera livshotande tillstånd.
        List<bool> rödaSvar = StällFrågor(_bedömare.RödaFrågor);

        // Om inget rött svar finns ställs de gula frågorna.
        List<bool> gulaSvar = rödaSvar.Contains(true)
            ? new List<bool>()
            : StällFrågor(_bedömare.GulaFrågor);

        // Bedömaren använder svaren för att avgöra patientens prioritetsnivå.
        BokningsPrioritet färg = _bedömare.Bedöm(rödaSvar, gulaSvar);

        // Röd prioritet innebär att patienten behöver ringa 112.
        if (färg == BokningsPrioritet.Röd)
        {
            Console.WriteLine("\n RÖD: Ring 112. Ambulansen skickas direkt. Ingen tid bokas i systemet.");
            return;
        }

        // Grön prioritet innebär att patienten inte bedöms behöva akut vård.
        if (färg == BokningsPrioritet.Grön)
        {
            Console.WriteLine("\n GRÖN: Inte akut. Patienten bokar en vanlig tid i patientmenyn.");
            return;
        }

        // Gul prioritet innebär att patienten behöver en akut tid samma dag.
        Console.WriteLine("\n GUL: Patienten ska ha en akut tid i dag.");

        // Hämtar en befintlig patient eller registrerar en ny akut patient.
        Patient? patient = HittaEllerRegistreraPatient();
        if (patient == null) return;

        // Försöker boka en akut tid genom bokningstjänsten.
        Bokning? bokning = _tjänst.BokaAkut(_akutTyp, vårdcentral, DateTime.Today, patient);

        // Hanterar situationen om inga akuta tider finns tillgängliga.
        if (bokning == null)
        {
            Console.WriteLine("\n Inga akuta tider kvar i dag på någon vårdcentral (öppet vardagar 08-17). " +
                              "Hänvisa till 1177 eller akutmottagningen.");
            return;
        }

        // Informerar om patienten har fått en tid på en annan vårdcentral.
        if (bokning.Personal.Vårdcentral.Id != vårdcentral.Id)
        {
            Console.WriteLine($"\n Ingen tid på {vårdcentral.Namn}. Patienten skickas till {bokning.Personal.Vårdcentral.Namn}.");
        }

        // Visar information om den skapade akuta bokningen.
        Console.WriteLine("\n Gul tid bokad: " + KonsolHjälp.BokningsText(bokning, true));
    }

    /// Ställer en lista med frågor och samlar in patientens ja- och nejsvar.
    /// Om patienten svarar ja avslutas frågorna för att gå vidare i bedömningen.
    private List<bool> StällFrågor(List<string> frågor)
    {
        List<bool> svar = new List<bool>();

        // Går igenom frågorna en i taget.
        foreach (string fråga in frågor)
        {
            Console.WriteLine("\n " + fråga);
            Console.WriteLine(" 1. Ja");
            Console.WriteLine(" 2. Nej");

            // Om användaren väljer 1 sparas svaret som true, annars false.
            bool ja = KonsolHjälp.LäsHeltal("Val: ", 1, 2) == 1;
            svar.Add(ja);

            // Avslutar frågorna om patienten svarar ja.
            if (ja) break;
        }

        return svar;
    }

    /// Söker efter en patient med hjälp av personnummer.
    /// Om patienten inte finns registrerad skapas en ny patient
    /// och sparas i patientregistret.
    private Patient? HittaEllerRegistreraPatient()
    {
        // Läser in patientens personnummer.
        string? personnummer = KonsolHjälp.LäsPersonnummer("Patientens personnummer (10 siffror, Enter = avbryt): ");
        if (personnummer == null) return null;

        // Söker efter patienten i repositoryt.
        Patient? patient = _patienter.Hämta(personnummer);

        // Returnerar patienten direkt om den redan finns registrerad.
        if (patient != null) return patient;

        // Om patienten inte hittas läses namn in för registrering.
        Console.WriteLine("Patienten finns inte, registrera nu: ");
        string förnamn = KonsolHjälp.LäsNamn("Förnamn: ");
        string efternamn = KonsolHjälp.LäsNamn("Efternamn: ");

        // Skapar ett nytt patientobjekt.
        patient = new Patient(förnamn, efternamn, personnummer);

        // Sparar den nya patienten i patientregistret.
        _patienter.LäggTill(patient);

        return patient;
    }

    /// Låter personalen välja en aktiv bokning på vårdcentralen
    /// och ställa in besöket.
    private void StällIn(Vårdcentral vårdcentral)
    {
        Console.WriteLine("\n Vilket besök ska ställas in?");

        // Visar vårdcentralens aktiva bokningar och låter personalen välja en.
        Bokning? bokning = KonsolHjälp.VäljBokning(AktivaHär(vårdcentral), true);

        if (bokning == null)
            return;

        // Försöker ställa in bokningen genom bokningstjänsten.
        if (!_tjänst.StällIn(bokning.Id))
        {
            Console.WriteLine(" Bokningen gick inte att ställa in.");
        }
    }

    /// Låter personalen välja en aktiv bokning på vårdcentralen
    /// och avboka patientens besök.
    private void Avboka(Vårdcentral vårdcentral)
    {
        Console.WriteLine("\n Vilken bokning ska avbokas?");

        // Visar vårdcentralens aktiva bokningar och låter personalen välja en.
        Bokning? bokning = KonsolHjälp.VäljBokning(AktivaHär(vårdcentral), true);

        if (bokning == null)
            return;

        // Försöker avboka bokningen genom bokningstjänsten.
        if (!_tjänst.Avboka(bokning.Id))
        {
            Console.WriteLine(" Bokningen gick inte att avboka.");
        }
    }
}

