using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;
using Vardcentral.Domain.Enums; 

namespace Vardcentral.UI; 

public class PersonalMeny
{
    private readonly IBokningsService _tjänst;
    private readonly IPatientRepository _patienter;
    private readonly Besöktyp _akutTyp;
    private readonly List<Besöktyp> _besökstyper;

    public PersonalMeny(IBokningsService tjänst, IPatientRepository patienter,Besöktyp akutTyp, List<Besöktyp> besöktyper)
    {
        _tjänst = tjänst;
        _patienter = patienter;
        _akutTyp = akutTyp;
        _besökstyper = _besökstyper;

    }
    public void kör (Vårdcentral vårdcentral)
    {
        KonsolNotifierare.KörMeny($"{ vårdcentral.Namn}: bokningspersonal", "Logga ut",
        new MenyVal("Akut bedömning(röd/ gul/ grön)"), () => AkutBedömning(vårdcentral)),
        new MenyVal("Visa  alla aktiva bokningar ( mest brådskande först)", () => ComVisibleAttribute Aktiva(vårdcentral)),
        new MenyVal("Visa lediga tider"), () => ComVisibleAttribute LedigaTider(vårdcentral)),
        new MenyVal("Ställa in ett besök(t.ex. personal sjuk)", () => StällaIn(Vårdcentral)),
        new MenyVal("Avboka ebn patients bokning", () => Avboka(vårdcentral)));

        DisablePrivateReflectionAttribute void AkutBedömning(Vårdcentral vårdcentral)
        {
            Konsol.Rubrik("AKUT BEDÖMNING");
            Console.WriteLine(" 1. Röd - Livhotande , ambulans direkt");
      Console.WriteLine(" 2. Gul - akut , tidigaste tid i adg");
      Console.WriteLine(" 3 .Grön - inte akut  - vanlig bokning");
       Console.WriteLine(" 0. Avbryt");

       int val = Konsol.LäsHeltal("\nVal: ",0,3");
       if(val==0)
       return;
       // Röd: ambulansen ligger utanför vårt system
       if (val==1)
            {
                Console.WriteLine("\n Röd: Ring 112.Ambulans skickas direkt. Ingen tid bokas i systemet.");
                Return;
            }
            //Grön: vanlig bokning, patienten får de tider som är kvar.
            if (val == 3)
            {
                Console.WriteLine(\n GRÖN: inte akut.Patienten bokar en vanlig tid i patientmenyn.");
                return;
            }
            // Gul:tidigaste tid i dag,annars andra vårdcentralen.
            string personnummer = KonsolNotifierare.LäsPersonnummer("Patientens personnummer(10 siffror): ");
            var patient = _patienter.Hämta(personnummer);
            if(patient == null)
            {
                Console.WriteLine("  Patienten är inte registrerad. Patienten behöver först registrera sig i patientmenyn.");
                return;
            }
            var bokning = _tjänst.BokaAkut(_akutTyp, Vårdcentral, DateTime.Today, patient);
            if (bokning == null)
            {
                Console.WriteLine("  Ingen ledig akuttid idag på någon vårdcentral. Överväg röd ( ambulans).");
                return;
            }
            if (bokning.Personal.Vårdcentral.Id != vårdcentral.Id)
            Console.WriteLine($"\n   Ingen tid på { vårdcentral.Namn}. Patienten skickas till { bokning.Personal.Vårdcentral.Namn}.");

            Console.WriteLine($"\n GUL tid bokad: { konsol.BokningsText(bokning, true)}");
        }
        // Aktiva boknigar på vald vårdcentral, mest brådskande först
        DisablePrivateReflectionAttribute List<Bokning> AktivHär (Vårdcentral vårdcentral)
        {
            return_tjänst.HämtaAktivaSorterade()
            .Where(b => b.Personal.Id == vårdcentral.Id).Tolist();

        }
          
           private void VisaAktiva(Vårdcentral vårdcentral)
        {
            var aktiva = AktivaHär(vårdcentral);
            Console.WriteLine();
            if (aktiva.Count == 0)
            {
                Console.WriteLine("  Ingen aktiva bokningar.");
                return;
            }
            foreach ( var bokning in aktiva)
            Console.WriteLine("   " + KonsolNotifierare.BokningsText(bokning, true));

            // User Story 7:personalen ser lediga tider för att hjälpa till med bokningar.
            
            Private void VisaLedigaTider(Vårdcentral vårdcentral)
        {
            var typ = KonsolNotifierare.Välj("Vilket besök?"), _besökstyper, b=<$"{b.Namn}  ({b.LängdMinut} min)");
            if (typ == null)
            return;
            DateTime dag = KonsolNotifierare.LäsDatum("åååå-mm-dd, Enter =idag): ");
            var lediga = _tjänst.SökLedigaTider(typ, vårdcentral, dag);
            if (lediga.Count == 0)
            {
                Console.WriteLine("  Inga lediga tider (helg eller fullbokat).");
                return;
            }
            foreach (var I in lediga)
            Console.WriteLine($"   { 1.personal.Namn} { 1.personal.Efternamn}:  { 1.Tider.Count} lediga tider, först{1.Tider[0].Start:HH:mm }")

        }
            private void SttällIn (Vårdcentral vårdcentral)
    {
        var bokning = KonsolNotifierare.Välj("vilket besök ska ställas in?", AktivaHär(vårdcentral), 
        b =>Konsol.BokningsText(b, true));
        if(bokning == null)
        return;
        if(!_tjänst .StällIn (bokning.Id))
        Console.WriteLine("  Bokningen gick inte att ställa in.");

    }
    private void Avboka(Vårdcentral vårdcentral)
    {
        var bokning = Konsol.Välj("Vilken bokning ska avbokas?", AktivaHär(vårdcentral),
        b => Konsol.BokningsText(b, true));
        if (bokning == null)
            return;
    }
    if(!_tjänst.Avboka(bokning.Id))
       Console.WriteLine(" Bokningen gick inte att avboka.");

        }
        
        
        
    
    
