using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Interfaces;
using Vardcentral.Application.Services;
using Vardcentral.Application.Strategier;
using Vardcentral.Application.Infrastructure;
using Vardcentral.Application;
// två vårdcentraler
var norr = new Vårdcentral(1, "Vårdcentralen Norr");
var söder = new Vårdcentral(2, "Vårdcentralen Söder");
var Vårdcentraler = new List<Vårdcentral> { norr, söder };
// Rum kan delas av flera 
var rumNorr1 = new Rum(1, "Rum 1 (Norr)");
var rumNorr2 = new Rum(2, "Rum 2(Norr)");
var rumSöder1 = new Rum(3, "Rum 1 ( Söder)");
var rumSöder2 = new Rum(4, "Rum 2 ( Söder)");

var personalRepo = new  InMemoryPersonalRepository();
personalRepo.LäggTill(new Personal("Ahmad", "Anna", Personaltyp.Läkare, "100001", norr, rumNorr1));
personalRepo.LäggTill(new Personal("Sarra", "Diana", Personaltyp.Sjuksköterska, "100002", norr, rumNorr2));
personalRepo.LäggTill(new Personal("Adem", "Rena", Personaltyp.Fysioterapeut, "100003", norr, rumNorr2));
personalRepo.LäggTill(new Personal("Fanar", "Delina", Personaltyp.Läkare, "200001", söder, rumSöder2));
personalRepo.LäggTill(new Personal("Eva", "Fredik", Personaltyp.Sjuksköterska, "200002", söder, rumSöder2));
personalRepo.LäggTill(new Personal("Nour", "Aya", Personaltyp.Fysioterapeut, "200003", söder, rumSöder2));

// Våra antagnaden:fasta längder.
var besökstyper = new List<Besöktyp>
{
    new Besöktyp("Läkarbesök", 10, Personaltyp.Läkare),
    new Besöktyp("sjuksköterskebesök", 20, Personaltyp.Sjuksköterska),
    new Besöktyp("Fysioterapi", 45, Personaltyp.Fysioterapeut),
};
var akutTyp = new Besöktyp("Akut", 30, Personaltyp.Sjuksköterska);// personalen avgör akut

// Påhittade patienter.
var patienter = new List<Patient>{

    new Patient("Sara","Andersson", "0001011234"),
    new Patient("Omer","Ahmad", "0002025678")
 };

// koppla ihop klasserna//
var bokningRepo = new MinnesBokningRepository();
var tidsSökare = new TidsSökare(personalRepo, bokningRepo);
var notiser = new NotifieringsService();
notiser.Prenumerera(new KonsolNotifierare());// observer 
IBokningsService service = new BokningsService(bokningRepo, personalRepo, tidsSökare, notiser, new AkutFörstRegel());// Strategy

//---meny---//
bool kör = true;
while (kör)
{
    Console.WriteLine();
    Console.WriteLine("==Vårdcentralens bokningstjänst==");
    Console.WriteLine("patient:");
    Console.WriteLine(" 1. Sök lediga tider och boka");
    Console.WriteLine(" 2. Visa mina bokningar");
    Console.WriteLine(" 3. Avboka");
    Console.WriteLine("Personal:");
    Console.WriteLine(" 4. Ställa in  bokning ( t.ex personal sjuk)");
    Console.WriteLine(" 5. Visa alla aktiva bokningar ( akut först)");
    Console.WriteLine(" 0 . Avsluta");

    int val = LäsHeltal("Val: ", 0, 6);
    switch (val)
    {
        case 1: SökOchBoka(); break;
        case 2: VisaMinaBokningar(); break;
        case 3: AvbokaMeny(); break;
        case 4: BokaAkutMeny(); break;
        case 5: StällInMeny(); break;
        case 6: VisaAllaAktiva(); break;
        case 0: kör = false; break;
    }

}
//------4.Menyval--
void SökOchBoka()
{
    Patient patient = VäljPatient();
    Vårdcentral vc = VäljVårdcentral();

    for (int i = 0; i < besökstyper.Count; i++)
        Console.WriteLine($" {i + 1}. {besökstyper[i].Namn} ({besökstyper[i].LängdMinut})");
    Besöktyp typ = besökstyper[LäsHeltal("Besökstyper: ", 1, besökstyper.Count) - 1];

    DateTime dag = LäsDatum("åååå-mm-dd):");

    // två listor : vem och när, på samma plats
    var personer = new List<Personal>();
    var tider = new List<Tidslucka>();
    foreach (var lediga in service.SökLedigaTider(typ, vc, dag))
    {

        for (int i = 0; i < lediga.Tider.Count && i < 10; i++)  // högst 10 person
        {

            personer.Add(lediga.Personal);
            tider.Add(lediga.Tider[i]);
        }

    }
    if (tider.Count == 0)
    {
        Console.WriteLine("Inga lediga tider den dagen( helg eller fullbokat).");
        return;
    }
    for (int i = 0; i < tider.Count; i++)
        Console.WriteLine($" {i + 1}. {tider[i].Start:HH:mm}-{tider[i].Slut:HH:mm} hos {personer[i].Namn} {personer[i].Efternamn}");

    int nr = LäsHeltal("Välj tid ", 1, tider.Count) - 1;
    var bokning = service.SkapaBokning(typ, personer[nr], tider[nr], patient);
    if (bokning == null)
        Console.WriteLine("Tiden kunde inte bokas.");
    else
    {
        Console.WriteLine($"Klart! Ditt boknings-id är {bokning.Id}.");
    }
}

// visar patientens bokningar.
void VisaMinaBokningar()
{
    Patient patient = VäljPatient();
    var bokningar = service.HämtaBokningar(patient.Personnummer);
    if (bokningar.Count == 0)
        Console.WriteLine("Du har inag bokningar.");
    foreach (var b in bokningar)
        skrivBokning(b);
}
// Patienten avbokar med bokning -id.
void AvbokaMeny()
{
    int id = LäsHeltal("Boknings-id att avboka:", 1, int.MaxValue);
    if (!service.Avboka(id))
        Console.WriteLine("Hittade ingen aktiv bokning med det id:t.");
}
// Personal bokar akut : Första lediga tid idag.
void BokaAkutMeny()
{
    Patient patient = VäljPatient();
    Vårdcentral vc = VäljVårdcentral();
    var bokning = service.BokaAkut(akutTyp, vc, DateTime.Today, patient);
    if (bokning == null)
        Console.WriteLine("Ingen ledig akuttid i dag på den vårdcentralen.");

}
// Personalen ställer in : flytta till andra vårdcentralen eller blir inställd.
void StällInMeny()
{
    int id = LäsHeltal("Boknings-id att ställa in :", 1, int.MaxValue);
    if (!service.StällIn(id))
        Console.WriteLine("Hittade ingen  aktiv bokning med det id :t.");

}
// Visar alla aktiva bokningar, akut först 
  void VisaAllaAktiva()
{
    var aktiva = service.HämtaAktivaSorterade();
    if (aktiva.Count == 0)
        Console.WriteLine("inga aktiva bokningar.");
    foreach (var b in aktiva)
        skrivBokning(b);

}

//---Hjälpmetoder---
void skrivBokning(Bokning b)
{
    Console.WriteLine($" # {b.Id} {b.Tid.Start:yyyy-MM-dd HH:mm} {b.Typ.Namn} hos {b.Personal.Efternamn}"
    + $"({b.Personal.Vårdcentral.Namn},{b.Rum.Namn}) - {b.Patient.Namn} {b.Patient.Efternamn} - {b.Prioritet}");

}
// användaren väljer patient
Patient VäljPatient()
{
    for (int i = 0; i < patienter.Count; i++)
        Console.WriteLine($" {i + 1}. {patienter[i].Namn} {patienter[i].Efternamn}");
    return patienter[LäsHeltal("Patient: ", 1, patienter.Count) - 1];
}
// användaren väljer vårdcentral
Vårdcentral VäljVårdcentral()
{
    for (int i = 0; i < Vårdcentraler.Count; i++)
        Console.WriteLine($"{i + 1}. {Vårdcentraler[i].Namn}");
    return Vårdcentraler[LäsHeltal("Vårdcentral:  ", 1, Vårdcentraler.Count) - 1];

}

// Läser ett heltal.
int LäsHeltal(string fråga, int min, int max)
{
    while (true)
    {
        Console.Write(fråga);
        string? text = Console.ReadLine();
        if (int.TryParse(text, out int tal) && tal >= min && tal <= max)
            return tal;
        Console.WriteLine($" Skriv ett tal mellan {min} och {max}.");

    }

}
//Läser ett datum, idag eller senare
DateTime LäsDatum(string fråga)
{
    while (true)
    {
        Console.Write(fråga);
        string? text = Console.ReadLine();
        if (DateTime.TryParse(text, out DateTime datum) && datum.Date >= DateTime.Today)
            return datum.Date;
        Console.WriteLine("Skriv ett datum i formen 2026-11-14 eller senare).");
    }
}








