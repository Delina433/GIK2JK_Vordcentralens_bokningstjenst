using System.Runtime.CompilerServices;
using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;

namespace Vardcentral.UI;

public class PatientMeny
{
     private readonly IBokningsService __tjänst;
    private readonly IPatientRepository _patienter;
    private readonly List<Besöktyp>_besöktyper;
    private readonly List<Vårdcentral> _vårdcentraler;

    public PatientMeny(IBokningsService tjänst, IPatientRepository patienter, List<Besöktyp> besöktyper, List<Vårdcentral> vårdcentraler)
    {
        __tjänst = tjänst;
        _patienter = patienter;
        _besöktyper = besöktyper;
        _vårdcentraler = vårdcentraler;
    }

    public void kör(Vårdcentral vårdcentral)
    {
        bool klar = false;
        while (!klar)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn+ ": patient");
            Console.WriteLine(" 1. Boka besök ");
            Console.WriteLine(" 2. Visa mina bokningar");
            Console.WriteLine(" 3.Avboka min bokning ");
            Console.WriteLine(" 0.Tillbaka");

            int val = KonsolHjälp.LäsHeltal("\n Val: ",0,3);

            switch (val)
            {
                case 1: BokaTid(vårdcentral); break;
                case 2: VisaMinaBokningar(); break;
                case 3: Avboka();break;
                case 0: klar= true; break;
            }
        }
    }
       //-------Boka tider---------
    private void BokaTid(Vårdcentral vårdcentral)
    {
        // 1.Nya patienter registreras här.
        Patient? patient = HittaEllerRegistrera();

        if (patient == null)
        {
            return;
        }

        //2. Vilket besök
        Besöktyp? typ = VäljBesöktyp();

        if (typ == null)
        {
            return;
        }

        //3. Viken dag
        DateTime dag = KonsolHjälp.LäsDatum("Datum(åååå-mm-dd, Enter = idag): ");

        //4. Sök lediga tider hos vald vårdcentral.
        List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, vårdcentral, dag);

        if (lediga.Count == 0)
        {
            Console.WriteLine($"\n Inga lediga tider hos {vårdcentral.Namn} den {dag:yyyy-MM-dd}.");
            VisaAndraVårdcentraler(typ, vårdcentral,dag);
            return;
        }

        //5. Vem vill patientern träffa?
        LedigaTider? valdPersonal = VäljPerson(lediga);

        if (valdPersonal == null)
        return;

        //6. vilken tid
        Tidslucka? tid = VäljTid (valdPersonal.Tider);

        if (tid == null)
        {
            return;
        }

        //.Boka. ger null om tiden redan är tagen eller inte passar.
        Bokning? bokning = _tjänst.SkapaBokning(typ, valdPersonal.Personal, tid patient);

        if(bokning == null)
        {
            Console.WriteLine("Tiden kunde inte bokas. Försök igen.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($" Klrt! Ditt boknings-id är {bokning.Id}.");
        Console.WriteLine($"{bokning.Typ.Namn}, {bokning.Tid.Start:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"{vårdcentral.Namn}, {bokning.Rum.Namn}");
    }

    
    private Patient? HittaEllerRegistrera()
    {
        string? personnummer = KonsolHjälp.LäsPersonnummer("Ditt personnummer (10 siffror, Enter = avbryt): ");

        if (personnummer == null)
        {
            return null;
        }

        Patient? patient = _patienter.Hämta(personnummer);

        if (patient != null)
        {
            Console.WriteLine($"Hej {patient.Namn} {patient.Efternamn}");
            return patient;
        }

        //personnummer finns inte patienten är ny, eller så skriver hen fel.

        Console.WriteLine($"Vi hittar ingen personnummer{personnummer}.");
        Console.WriteLine(" 1. Jag är ny, jag vill registrera mig");
        Console.WriteLine(" 0. Avbryt");
        Console.WriteLine("\nVal:", 0,1 )== 0)
         
         return null;

         string förnamn = KonsolHjälp.LäsNamn("Förenamn: ");
         string efternamn = KonsolHjälp.LäsNamn("Efternamn: ");

         Patient ny = new Patient(förnamn, efternamn, personnummer);
         _patienter.LäggTill(ny);
         Console.WriteLine($"Välkommen {ny.Namn} {ny.Efternamn}! Du är nu registrerad.");
         return ny;
    }

    private Besöktyp? VäljBesöktyp()
    {
        Console.WriteLine("\nVilket besök gäller det?");
        
        for (int i=0; i<_besöktyper.Count; i++)
        {
            Console.WriteLine($"Välkommen {ny.Namn} {ny.Efternamn}! Du är nu registrerad.");
            return ny;
        }

    }
    private Besöktyp? VäljBesöktyp()
    {
        Console.WriteLine("\n Vilket besök gäller det?");

        for (int i = 0; i < _besöktyper.Count; i++)
        {
            Console.WriteLine($"{i +1}. {_besöktyper[i].Namn}({_besöktyper[i].LängdMinut}min)");
        }
        Console.WriteLine(" 0.Avbryt");

        int val = KonsolHjälp.LäsHeltal ("\nVal:", 0,_besöktyper.Count);

        if (val==0)
        {
            return null;
        }

        return _besöktyper[val -1];
    }
    private LedigaTider? VäljPerson(List<LedigaTider> lediga)
    {
        Console.WriteLine("\n Vilken personal vill du träffa?");
        for(int i = 0; i < lediga.Count; i++)
        {
            Personal p = lediga[i].Personal;
            Console.WriteLine($" {i + 1}. {p.Namn} ({p.Typ},{p.Rum.Namn})," +
            $"  första lediga tid{lediga[i].Tid[0].Start:yyyy-MM-dd HH:mm}");
             {p.Vårdcentral.Namn}");
        }
       Console.WriteLine(" 0. Avbryt");

        int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, lediga.Count);

        if (val == 0)
        {
            return null;
        }

        return lediga[val - 1];
    }
     private Tidslucka? VäljTid(List<Tidslucka> tider)
    {
        List<Tidslucka> sorterade = new List<Tidslucka>(tider);
        foreach (Tidslucka tid in tider)

        {
            if (tid.Start .Minute %15 == 0 && visade.Count< 12)
            visade.Add(tid);
            
            {
                if(visade.Count == 0)
                {
                  for(int i = 0; i <tider.Count && i < 12; i++)
                  visade.Add(tider[i]);
                }
                  Console.WriteLine("\n Ledig tider :");
                  for (int i = 0; i < visade.Count; i++)
                  {
                    Console.WriteLine($" {i + 1}. {visade[i].Start:HH:mm} 
                    Console.WriteLine( 0. Avbryt");
                    int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, visade.Count);
                    if (val == 0)
                    {
                        return null;
                        return visade[val - 1]; 
                    }
                  privte void VisaAndraVårdcentraler(Besöktyp typ, Vårdcentral nuvarande, DateTime dag)
        {
           foreach (Vårdcentral annan in  _vårdcentraler)
            {
                if (annan.Id == nuvarande.Id) 
                continue;

                List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, annan, dag);

                if (lediga.Count == 0)
                 continue;
                 List<Tidslucka> ledig  = _Tjänst.SökledigaTider(typ, annan, dag);
                 if (ledig.Count == 0)
                    continue;
                    DateTime tidigaste = ledig[0].Start;
                    foreach(LedigaTider l in lediga)
                    {
                        if (l.Tider.Count == 0)
                         continue;
                        if (l.Tider[0].Start < tidigaste)
                        
                            tidigaste = l.Tider[0].Start;
                        }
                    }

                Console.WriteLine($"\n Det finns lediga tider hos {annan.Namn} , tidigaste kl  tid är {tidigaste: HH:mm}."+
                "Gå tillbabaka och välja den vårdcentralen för att boka där.");

                
            }
                  }
                  private void VisaBokningar ()
    {   
        Patient? patient = HittaEllerRegistrera();
        if (patient == null)
        {
            return;
            List<Bokning> mina = _tjänst.HämtaBokningarFörPatient(patient.Personnummer);
            mina.Sort((a, b) => a.Tid.Start.CompareTo(b.Tid.Start));
            console.WriteLine();
            if (mina.Count == 0)
            {
                Console.WriteLine("  Du har inga bokningar.");
                return;
            }
            foreach (Bokning bokning in mina)
            {
                Console.WriteLine("  " + KonsolHjälp.BokningsText(bokning, false));
            }
            private void Avboka()
            {
                Patient? patient = HittaBeFintPatient();
                if (patient == null)
                {
                    return;
                }
                List<Bokning> aktiva= new List<Bokning>();
                foreach (Bokning bokning in _tjänst.HämtaBokningar(patient.Personnummer))
                {
                    if (bokning.ÄrAktiv)
                    {
                        aktiva.Add(bokning);
                    }
                    aktiva.Sort((a, b) => a.Tid.Start.CompareTo(b.Tid.Start));
                    Console.WriteLine("\n  Vilken bokning vill du avboka?");
                    Bokning? bokning = KonsolHjälp.VäljBokning(aktiva, false);
                    if(vald == null)
                    {
                        return;
                        if (!_tjänst.Avboka(vald.Id))
                        {
                            Console.WriteLine("  Bokningen gick inte att avboka.");
                        }

                    }
                }
            }
                    
