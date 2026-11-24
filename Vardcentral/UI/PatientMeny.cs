using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;

namespace Vardcentral.UI;

/// Hanterar patientens meny och de funktioner som patienten kan utföra,
/// exempelvis boka besök, visa bokningar, avboka och läsa notiser.
public class PatientMeny
{
    // Interface som ger tillgång till patientrelaterade funktioner.
    private readonly IPatientService _tjänst;

    // Lista över de olika besökstyper som patienten kan välja mellan.
    private readonly List<Besöktyp> _besöktyper;

    // Lista över alla vårdcentraler som finns i systemet.
    private readonly List<Vårdcentral> _vårdcentraler;

    /// Skapar en PatientMeny och tar emot de beroenden som klassen behöver.
    public PatientMeny(IPatientService tjänst, List<Besöktyp> besöktyper, List<Vårdcentral> vårdcentraler)
    {
        // Sparar de beroenden som skickas in via konstruktorn.
        _tjänst = tjänst;
        _besöktyper = besöktyper;
        _vårdcentraler = vårdcentraler;
    }

    /// Startar patientmenyn för den valda vårdcentralen.
    /// Visar menyalternativ tills användaren väljer att gå tillbaka.
    public void Kör(Vårdcentral vårdcentral)
    {
        // Styr menyns loop. Blir true när användaren väljer att avsluta.
        bool klar = false;

        while (!klar)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn + ": patient");

            Console.WriteLine("\n Behöver du akut hjälp? Kontakta personalen. Vid livsfara: ring 112.");
            Console.WriteLine();
            Console.WriteLine(" 1. Boka besök ");
            Console.WriteLine(" 2. Visa mina bokningar");
            Console.WriteLine(" 3. Avboka min bokning ");
            Console.WriteLine(" 4. Mina notiser");
            Console.WriteLine(" 0. Tillbaka");

            // Läser in ett val mellan 0 och 4.
            int val = KonsolHjälp.LäsHeltal("\n Val: ", 0, 4);

            // Anropar rätt metod beroende på patientens val.
            switch (val)
            {
                case 1: BokaTid(vårdcentral); break;
                case 2: VisaBokningar(); break;
                case 3: Avboka(); break;
                case 4: VisaNotiser(); break;
                case 0: klar = true; break;
            }
        }
    }

    // ---------------- Boka tider 

    /// Hanterar hela bokningsprocessen från att hitta patienten
    /// till att skapa en bokning.

    private void BokaTid(Vårdcentral vårdcentral)
    {
        // 1. Hitta en befintlig patient eller registrera en ny.
        Patient? patient = HittaEllerRegistrera();

        // Avbryt om patienten väljer att inte fortsätta.
        if (patient == null) return;

        // 2. Låt patienten välja vilken typ av besök som önskas.
        Besöktyp? typ = VäljBesöktyp();

        if (typ == null) return;

        // 3. Läs in önskat datum.
        DateTime dag = KonsolHjälp.LäsDatum("Datum(åååå-mm-dd, Enter = idag): ");

        // 4. Sök efter lediga tider för den valda besökstypen,
        // vårdcentralen och dagen.
        List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, vårdcentral, dag);

        // Om inga tider finns visas ett meddelande och andra vårdcentraler kontrolleras.
        if (lediga.Count == 0)
        {
            Console.WriteLine($"\n Inga lediga tider hos {vårdcentral.Namn} den {dag:yyyy-MM-dd} (öppet vardagar 08-17).");
            VisaAndraVårdcentraler(typ, vårdcentral, dag);
            return;
        }

        // 5. Låt patienten välja vilken personal hen vill träffa.
        LedigaTider? valdPersonal = VäljPerson(lediga);

        if (valdPersonal == null) return;

        // 6. Låt patienten välja en av personalens lediga tider.
        Tidslucka? tid = KonsolHjälp.VäljTid(valdPersonal.Tider);

        if (tid == null) return;

        // 7. Försök skapa bokningen genom servicen.
        // Returnerar null om tiden redan är bokad eller inte längre är tillgänglig.
        Bokning? bokning = _tjänst.SkapaBokning(typ, valdPersonal.Personal, tid, patient);

        if (bokning == null)
        {
            Console.WriteLine("Tiden kunde inte bokas. Försök igen.");
            return;
        }

        // Visar information om bokningen om den skapades.
        Console.WriteLine();
        Console.WriteLine($" Klart! Ditt boknings-id är {bokning.Id}.");
        Console.WriteLine($"{bokning.Typ.Namn}, {bokning.Tid.Start:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"{vårdcentral.Namn}, {bokning.Rum.Namn}");
    }

    /// Söker efter en befintlig patient med personnummer.
    /// Om patienten inte finns kan personen välja att registrera sig.
    private Patient? HittaEllerRegistrera()
    {
        // Läser in personnummer. Null innebär att användaren avbryter.
        string? personnummer = KonsolHjälp.LäsPersonnummer("Ditt personnummer (10 siffror, Enter = avbryt): ");

        if (personnummer == null) return null;

        // Kontrollerar om patienten redan finns genom servicen.
        Patient? patient = _tjänst.HämtaPatient(personnummer);

        if (patient != null)
        {
            Console.WriteLine($"Hej {patient.Namn} {patient.Efternamn}");
            return patient;
        }

        // Om patienten inte hittas får personen möjlighet att registrera sig.
        Console.WriteLine();
        Console.WriteLine($"Vi hittar inget personnummer: {personnummer}.");
        Console.WriteLine();
        Console.WriteLine(" 1. Jag är ny, jag vill registrera mig");
        Console.WriteLine(" 0. Avbryt");

        int val = KonsolHjälp.LäsHeltal("Val: ", 0, 1);

        if (val == 0) return null;

        // Hämtar namnuppgifter från den nya patienten.
        string förnamn = KonsolHjälp.LäsNamn("Förnamn: ");
        string efternamn = KonsolHjälp.LäsNamn("Efternamn: ");

        // Registrerar patienten genom servicen.
        Patient ny = _tjänst.RegistreraPatient(förnamn, efternamn, personnummer);

        Console.WriteLine($"Välkommen {ny.Namn} {ny.Efternamn}! Du är nu registrerad.");

        return ny;
    }

    /// Visar alla tillgängliga besökstyper och låter patienten välja en.
    private Besöktyp? VäljBesöktyp()
    {
        Console.WriteLine("\n Vilket besök gäller det?");

        // Går igenom listan och skriver ut varje besökstyp.
        for (int i = 0; i < _besöktyper.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_besöktyper[i].Namn}({_besöktyper[i].LängdMinut}min)");
        }

        Console.WriteLine(" 0. Avbryt");

        // Användaren kan välja ett nummer som motsvarar en besökstyp.
        int val = KonsolHjälp.LäsHeltal("\nVal:", 0, _besöktyper.Count);

        if (val == 0) return null;

        // Listor börjar på index 0, därför subtraheras 1 från användarens val.
        return _besöktyper[val - 1];
    }

    /// Visar personal som har lediga tider och låter patienten välja.
    private LedigaTider? VäljPerson(List<LedigaTider> lediga)
    {
        Console.WriteLine("\n Vilken personal vill du träffa?");

        // Går igenom personalen som har lediga tider.
        for (int i = 0; i < lediga.Count; i++)
        {
            Personal p = lediga[i].Personal;

            // Visar namn, yrkesroll och rum för varje person.
            Console.WriteLine($" {i + 1}. {p.Namn} {p.Efternamn} ({p.Typ},{p.Rum.Namn}),");
        }

        Console.WriteLine(" 0. Avbryt");

        // Läser in patientens val.
        int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, lediga.Count);

        if (val == 0) return null;

        // Returnerar den valda personens information och lediga tider.
        return lediga[val - 1];
    }

    /// Söker efter lediga tider hos andra vårdcentraler
    /// om den valda vårdcentralen saknar lediga tider.
    private void VisaAndraVårdcentraler(Besöktyp typ, Vårdcentral nuvarande, DateTime dag)
    {
        // Går igenom samtliga vårdcentraler.
        foreach (Vårdcentral annan in _vårdcentraler)
        {
            // Hoppar över vårdcentralen som redan har kontrollerats.
            if (annan.Id == nuvarande.Id) continue;

            // Söker efter lediga tider hos den andra vårdcentralen.
            List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, annan, dag);

            // Fortsätter till nästa vårdcentral om inga tider finns.
            if (lediga.Count == 0) continue;

            // Varje persons lista är sorterad, så den första tiden är den tidigaste.
            // Min hämtar den tidigaste tiden bland personalens första lediga tider.
            DateTime tidigast = lediga.Min(l => l.Tider[0].Start);

            Console.WriteLine($" Det finns lediga tider hos {annan.Namn} , tidigast kl. {tidigast:HH:mm}." +
                "Gå tillbaka och välj den vårdcentralen för att boka där.");
        }
    }

    // ---------------- Visa och avboka 

    /// Hämtar och visar patientens bokningar sorterade efter starttid.
    private void VisaBokningar()
    {
        // Hittar en befintlig patient eller erbjuder registrering.
        Patient? patient = HittaEllerRegistrera();

        if (patient == null) return;

        // Hämtar patientens bokningar och sorterar dem med den tidigaste först.
        List<Bokning> mina = _tjänst.HämtaBokningar(patient.Personnummer)
                                    .OrderBy(b => b.Tid.Start)
                                    .ToList();

        // Skriver ut bokningarna. Visar meddelande om listan är tom.
        KonsolHjälp.SkrivBokningar(mina, false, "Du har inga bokningar.");
    }

    /// Hämtar patientens aktiva bokningar och låter patienten välja
    /// vilken bokning som ska avbokas.
    private void Avboka()
    {
        // Hämtar en redan registrerad patient.
        Patient? patient = HittaBefintligPatient();

        if (patient == null) return;

        // Hämtar patientens bokningar, filtrerar bort inaktiva bokningar
        // och sorterar de återstående efter starttid.
        List<Bokning> aktiva = _tjänst.HämtaBokningar(patient.Personnummer)
                                      .Where(b => b.ÄrAktiv)
                                      .OrderBy(b => b.Tid.Start)
                                      .ToList();

        Console.WriteLine("\n Vilken bokning vill du avboka?");

        // Låter patienten välja en bokning från listan.
        Bokning? vald = KonsolHjälp.VäljBokning(aktiva, false);

        if (vald == null) return;

        // Avbokar bokningen genom servicen.
        // Observer-mönstret används för att skicka en notis om avbokningen.
        if (!_tjänst.Avboka(vald.Id))
        {
            Console.WriteLine("  Bokningen gick inte att avboka.");
        }
    }

    /// Söker efter en redan registrerad patient med personnummer.
    /// Registrerar inte nya patienter.
    private Patient? HittaBefintligPatient()
    {
        // Läser in patientens personnummer.
        string? personnummer = KonsolHjälp.LäsPersonnummer("Ditt personnummer (10 siffror, Enter = avbryt): ");

        if (personnummer == null) return null;

        // Hämtar patienten från servicen.
        Patient? patient = _tjänst.HämtaPatient(personnummer);

        // Informerar användaren om patienten inte finns.
        if (patient == null)
        {
            Console.WriteLine(" Vi hittar ingen patient med det personnumret. " +
                                "Boka en tid först för att registrera dig.");
        }

        return patient;
    }

    /// Hämtar och visar alla notiser som tillhör patienten.
    private void VisaNotiser()
    {
        // Identifierar patienten genom personnummer.
        Patient? patient = HittaBefintligPatient();

        if (patient == null) return;

        // Hämtar patientens notiser från servicen.
        List<string> notiser = _tjänst.HämtaNotiser(patient.Personnummer);

        Console.WriteLine();

        // Visar ett meddelande om patienten inte har några notiser.
        if (notiser.Count == 0)
        {
            Console.WriteLine("Du har inga notiser.");
            return;
        }

        // Skriver ut varje notis i listan.
        foreach (string notis in notiser)
        {
            Console.WriteLine("-" + notis);
        }
    }
}

 
