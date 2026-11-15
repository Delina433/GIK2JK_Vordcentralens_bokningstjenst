using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;

namespace Vardcentral.UI;

/// Hanterar patientens meny och de funktioner som patienten kan använda,
/// exempelvis att boka, visa och avboka besök samt läsa notiser.

public class PatientMeny
{
    // Används för att kommunicera med applikationens patienttjänster.
    private readonly IPatientService _tjänst;

    // Innehåller de olika typer av besök som patienten kan välja mellan.
    private readonly List<Besöktyp> _besöktyper;

    // Innehåller alla tillgängliga vårdcentraler.
    private readonly List<Vårdcentral> _vårdcentraler;

    /// Skapar patientmenyn och tar emot de tjänster och listor
    /// som behövs för att hantera patientens funktioner.
    public PatientMeny(IPatientService tjänst, List<Besöktyp> besöktyper, List<Vårdcentral> vårdcentraler)
    {
        _tjänst = tjänst;
        _besöktyper = besöktyper;
        _vårdcentraler = vårdcentraler;
    }

    /// Visar patientens huvudmeny och låter patienten välja
    /// vilken funktion som ska användas.
    public void Kör(Vårdcentral vårdcentral)
    {
        bool klar = false;

        // Menyn fortsätter visas tills patienten väljer att gå tillbaka.
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

            // Läser in och kontrollerar patientens menyval.
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

    /// Hanterar hela bokningsprocessen från registrering
    /// av patient till val av besökstyp, personal och ledig tid.
    private void BokaTid(Vårdcentral vårdcentral)
    {
        // 1. Hämtar en befintlig patient eller registrerar en ny patient.
        Patient? patient = HittaEllerRegistrera();
        if (patient == null) return;

        // 2. Låter patienten välja vilken typ av besök som önskas.
        Besöktyp? typ = VäljBesöktyp();
        if (typ == null) return;

        // 3. Läser in önskat datum för besöket.
        DateTime dag = KonsolHjälp.LäsDatum("Datum(åååå-mm-dd, Enter = idag): ");

        // 4. Söker efter lediga tider hos den valda vårdcentralen.
        List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, vårdcentral, dag);

        // Om inga tider finns visas ett meddelande och andra vårdcentraler kontrolleras.
        if (lediga.Count == 0)
        {
            Console.WriteLine($"\n Inga lediga tider hos {vårdcentral.Namn} den {dag:yyyy-MM-dd} (öppet vardagar 08-17).");
            VisaAndraVårdcentraler(typ, vårdcentral, dag);
            return;
        }

        // 5. Låter patienten välja vilken personal den vill träffa.
        LedigaTider? valdPersonal = VäljPerson(lediga);
        if (valdPersonal == null) return;

        // 6. Låter patienten välja en av personalens lediga tider.
        Tidslucka? tid = KonsolHjälp.VäljTid(valdPersonal.Tider);
        if (tid == null) return;

        // 7. Försöker skapa bokningen genom patienttjänsten.
        // Returnerar null om tiden redan är bokad eller inte längre är tillgänglig.
        Bokning? bokning = _tjänst.SkapaBokning(typ, valdPersonal.Personal, tid, patient);

        if (bokning == null)
        {
            Console.WriteLine("Tiden kunde inte bokas. Försök igen.");
            return;
        }

        // Visar bekräftelse och information om den skapade bokningen.
        Console.WriteLine();
        Console.WriteLine($" Klart! Ditt boknings-id är {bokning.Id}.");
        Console.WriteLine($"{bokning.Typ.Namn}, {bokning.Tid.Start:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"{vårdcentral.Namn}, {bokning.Rum.Namn}");
    }

    /// Söker efter en patient med hjälp av personnummer.
    /// Om patienten inte finns registrerad erbjuds möjligheten
    /// att skapa en ny patient.
    private Patient? HittaEllerRegistrera()
    {
        // Läser in personnummer. Om användaren avbryter returneras null.
        string? personnummer = KonsolHjälp.LäsPersonnummer("Ditt personnummer (10 siffror, Enter = avbryt): ");
        if (personnummer == null) return null;

        // Söker efter en befintlig patient genom patienttjänsten.
        Patient? patient = _tjänst.HämtaPatient(personnummer);

        if (patient != null)
        {
            Console.WriteLine($"Hej {patient.Namn} {patient.Efternamn}");
            return patient;
        }

        // Om personnumret inte hittas erbjuds användaren att registrera sig.
        Console.WriteLine();
        Console.WriteLine($"Vi hittar inget personnummer: {personnummer}.");
        Console.WriteLine();
        Console.WriteLine(" 1. Jag är ny, jag vill registrera mig");
        Console.WriteLine(" 0. Avbryt");

        int val = KonsolHjälp.LäsHeltal("Val: ", 0, 1);
        if (val == 0) return null;

        // Läser in den nya patientens namn.
        string förnamn = KonsolHjälp.LäsNamn("Förnamn: ");
        string efternamn = KonsolHjälp.LäsNamn("Efternamn: ");

        // Registrerar patienten genom patienttjänsten.
        Patient ny = _tjänst.RegistreraPatient(förnamn, efternamn, personnummer);

        Console.WriteLine($"Välkommen {ny.Namn} {ny.Efternamn}! Du är nu registrerad.");
        return ny;
    }

    /// Visar tillgängliga besökstyper och låter patienten välja en.
    private Besöktyp? VäljBesöktyp()
    {
        Console.WriteLine("\n Vilket besök gäller det?");

        // Skriver ut alla besökstyper med namn och besökets längd.
        for (int i = 0; i < _besöktyper.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_besöktyper[i].Namn}({_besöktyper[i].LängdMinut}min)");
        }

        Console.WriteLine(" 0. Avbryt");

        // Läser in patientens val.
        int val = KonsolHjälp.LäsHeltal("\nVal:", 0, _besöktyper.Count);

        if (val == 0) return null;

        // Returnerar den valda besökstypen från listan.
        return _besöktyper[val - 1];
    }

    /// Visar personal som har lediga tider och låter patienten välja
    /// vem den vill träffa.
    private LedigaTider? VäljPerson(List<LedigaTider> lediga)
    {
        Console.WriteLine("\n Vilken personal vill du träffa?");

        // Skriver ut tillgänglig personal med yrkesroll och rum.
        for (int i = 0; i < lediga.Count; i++)
        {
            Personal p = lediga[i].Personal;
            Console.WriteLine($" {i + 1}. {p.Namn} {p.Efternamn} ({p.Typ},{p.Rum.Namn}),");
        }

        Console.WriteLine(" 0. Avbryt");

        // Läser in patientens val.
        int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, lediga.Count);

        if (val == 0) return null;

        // Returnerar den valda personalens information och lediga tider.
        return lediga[val - 1];
    }

    /// <summary>
    /// Söker efter lediga tider hos andra vårdcentraler
    /// om den valda vårdcentralen saknar tillgängliga tider.
    /// </summary>
    /// <param name="typ">Den önskade besökstypen.</param>
    /// <param name="nuvarande">Den vårdcentral som redan har kontrollerats.</param>
    /// <param name="dag">Datumet som sökningen gäller.</param>
    private void VisaAndraVårdcentraler(Besöktyp typ, Vårdcentral nuvarande, DateTime dag)
    {
        // Går igenom samtliga vårdcentraler för att hitta alternativa tider.
        foreach (Vårdcentral annan in _vårdcentraler)
        {
            // Hoppar över vårdcentralen som redan har kontrollerats.
            if (annan.Id == nuvarande.Id) continue;

            // Söker efter lediga tider hos den andra vårdcentralen.
            List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, annan, dag);

            // Fortsätter till nästa vårdcentral om det inte finns några lediga tider.
            if (lediga.Count == 0) continue;

            // Listan är sorterad så den tidigaste tiden kan hämtas från första
            // lediga tidsluckan hos varje personal.
            DateTime tidigast = lediga.Min(l => l.Tider[0].Start);

            // Informerar patienten om var det finns alternativa tider.
            Console.WriteLine($" Det finns lediga tider hos {annan.Namn}, tidigast kl. {tidigast:HH:mm}." +
                " Gå tillbaka och välj den vårdcentralen för att boka där.");
        }
    }

    // ---------------- Visa och avboka bokningar ----------------

    /// <summary>
    /// Hämtar och visar patientens bokningar sorterade efter datum,
    /// med den tidigaste bokningen först.
    /// </summary>
    private void VisaBokningar()
    {
        // Hämtar patienten genom personnummer.
        Patient? patient = HittaEllerRegistrera();
        if (patient == null) return;

        // Hämtar patientens bokningar och sorterar dem efter starttid.
        List<Bokning> mina = _tjänst.HämtaBokningar(patient.Personnummer)
                                    .OrderBy(b => b.Tid.Start)
                                    .ToList();

        // Skriver ut bokningarna eller ett meddelande om listan är tom.
        KonsolHjälp.SkrivBokningar(mina, false, "Du har inga bokningar.");
    }

    /// Hämtar patientens aktiva bokningar och låter patienten
    /// välja vilken bokning som ska avbokas.
    private void Avboka()
    {
        // Hämtar en redan registrerad patient.
        Patient? patient = HittaBefintligPatient();
        if (patient == null) return;

        // Hämtar endast aktiva bokningar och sorterar dem efter starttid.
        // Redan avbokade eller inställda bokningar kan inte avbokas igen.
        List<Bokning> aktiva = _tjänst.HämtaBokningar(patient.Personnummer)
                                      .Where(b => b.ÄrAktiv)
                                      .OrderBy(b => b.Tid.Start)
                                      .ToList();

        Console.WriteLine("\n Vilken bokning vill du avboka?");

        // Låter patienten välja vilken bokning som ska avbokas.
        Bokning? vald = KonsolHjälp.VäljBokning(aktiva, false);

        if (vald == null) return;

        // Försöker avboka bokningen genom patienttjänsten.
        // En notis om avbokningen skapas genom Observer-mönstret.
        if (!_tjänst.Avboka(vald.Id))
        {
            Console.WriteLine("  Bokningen gick inte att avboka.");
        }
    }

    /// Hämtar en redan registrerad patient med hjälp av personnummer.
    /// Till skillnad från HittaEllerRegistrera skapas ingen ny patient.
    private Patient? HittaBefintligPatient()
    {
        string? personnummer = KonsolHjälp.LäsPersonnummer("Ditt personnummer (10 siffror, Enter = avbryt): ");

        if (personnummer == null) return null;

        // Söker efter patienten i systemet.
        Patient? patient = _tjänst.HämtaPatient(personnummer);

        // Informerar användaren om personnumret inte finns registrerat.
        if (patient == null)
        {
            Console.WriteLine(" Vi hittar ingen patient med det personnumret. " +
                              "Boka en tid först för att registrera dig.");
        }

        return patient;
    }

    /// Hämtar och visar patientens notiser.
    /// Om patienten saknar notiser visas ett meddelande om detta.
    private void VisaNotiser()
    {
        // Hämtar en befintlig patient genom personnummer.
        Patient? patient = HittaBefintligPatient();
        if (patient == null) return;

        // Hämtar patientens notiser genom patienttjänsten.
        List<string> notiser = _tjänst.HämtaNotiser(patient.Personnummer);

        Console.WriteLine();

        // Kontrollerar om patienten har några notiser.
        if (notiser.Count == 0)
        {
            Console.WriteLine("Du har inga notiser.");
            return;
        }

        // Skriver ut samtliga notiser.
        foreach (string notis in notiser)
        {
            Console.WriteLine("-" + notis);
        }
    }
}
