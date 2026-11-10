using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;
namespace Vardcentral.UI;

public class PatientMeny
{
     private readonly IPatientService _tjänst;
    private readonly List<Besöktyp>_besöktyper;
    private readonly List<Vårdcentral> _vårdcentraler;

    public PatientMeny(IPatientService tjänst, List<Besöktyp> besöktyper, List<Vårdcentral> vårdcentraler)
    {
        _tjänst = tjänst;
        _besöktyper = besöktyper;
        _vårdcentraler = vårdcentraler;
    }

    public void Kör(Vårdcentral vårdcentral)
    {
        bool klar = false;
        while (!klar)
        {
            KonsolHjälp.Rubrik(vårdcentral.Namn+ ": patient");
            Console.WriteLine("\n Behöver du akut hjälp? Kontakta personalen. Vid livsfara: ring 112.");
            Console.WriteLine();
            Console.WriteLine(" 1. Boka besök ");
            Console.WriteLine(" 2. Visa mina bokningar");
            Console.WriteLine(" 3. Avboka min bokning ");
            Console.WriteLine(" 4. Mina notiser");
            Console.WriteLine(" 0. Tillbaka");

            int val = KonsolHjälp.LäsHeltal("\n Val: ",0,4);

            switch (val)
            {
                case 1: BokaTid(vårdcentral); break;
                case 2: VisaBokningar(); break;
                case 3: Avboka();break;
                case 4: VisaNotiser(); break;
                case 0: klar= true; break;
            }
        }
    }
   
       //-------Boka tider---------
    private void BokaTid(Vårdcentral vårdcentral)
    {
        // 1.Nya patienter registreras här.
        Patient? patient = HittaEllerRegistrera();
        if (patient == null) return;

        //2. Vilket besök
        Besöktyp? typ = VäljBesöktyp();

        if (typ == null) return;

        //3. Viken dag
        DateTime dag = KonsolHjälp.LäsDatum("Datum(åååå-mm-dd, Enter = idag): ");

        //4. Sök lediga tider hos vald vårdcentral.
        List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, vårdcentral, dag);

        if (lediga.Count == 0)
        {
            Console.WriteLine($"\n Inga lediga tider hos {vårdcentral.Namn} den {dag:yyyy-MM-dd} (öppet vardagar 08-17).");
            VisaAndraVårdcentraler(typ, vårdcentral,dag);
            return;
        }

        //5. Vem vill patientern träffa?
        LedigaTider? valdPersonal = VäljPerson(lediga);
        if (valdPersonal == null) return;

        //6. vilken tid
        Tidslucka? tid = KonsolHjälp.VäljTid(valdPersonal.Tider);
        if (tid == null) return;
        

        //.Boka. ger null om tiden redan är tagen eller inte passar.
        Bokning? bokning = _tjänst.SkapaBokning(typ, valdPersonal.Personal, tid, patient);

        if(bokning == null)
        {
            Console.WriteLine("Tiden kunde inte bokas. Försök igen.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($" Klart! Ditt boknings-id är {bokning.Id}.");
        Console.WriteLine($"{bokning.Typ.Namn}, {bokning.Tid.Start:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"{vårdcentral.Namn}, {bokning.Rum.Namn}");
    }

    private Patient? HittaEllerRegistrera()
    {
        string? personnummer = KonsolHjälp.LäsPersonnummer("Ditt personnummer (10 siffror, Enter = avbryt): ");
        if (personnummer == null) return null;
        
        Patient? patient = _tjänst.HämtaPatient(personnummer);

        if (patient != null)
        {
            Console.WriteLine($"Hej {patient.Namn} {patient.Efternamn}");
            return patient;
        }

        //personnummer finns inte patienten är ny, eller så skriver hen fel.
        Console.WriteLine();
        Console.WriteLine($"Vi hittar inget personnummer: {personnummer}.");
        Console.WriteLine();
        Console.WriteLine(" 1. Jag är ny, jag vill registrera mig");
        Console.WriteLine(" 0. Avbryt");

        int val = KonsolHjälp.LäsHeltal("Val: ",0,1);
         if(val ==0) return null;
        
    
        string förnamn = KonsolHjälp.LäsNamn("Förnamn: ");
        string efternamn = KonsolHjälp.LäsNamn("Efternamn: ");

        Patient ny = _tjänst.RegistreraPatient(förnamn, efternamn, personnummer);
        Console.WriteLine($"Välkommen {ny.Namn} {ny.Efternamn}! Du är nu registrerad.");
        return ny;
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

        if (val==0) return null;
        

        return _besöktyper[val -1];
    }
    private LedigaTider? VäljPerson(List<LedigaTider> lediga)
    {
        Console.WriteLine("\n Vilken personal vill du träffa?");
        for(int i = 0; i < lediga.Count; i++)
        {
            Personal p = lediga[i].Personal;
            Console.WriteLine($" {i + 1}. {p.Namn} {p.Efternamn} ({p.Typ},{p.Rum.Namn})," );//+
                              //$"första lediga tid{lediga[i].Tider[0].Start:HH:mm}");
            
        }
       Console.WriteLine(" 0. Avbryt");

        int val = KonsolHjälp.LäsHeltal("\nVal: ", 0, lediga.Count);

        if (val == 0) return null;
        
        return lediga[val - 1];
    }

    private void VisaAndraVårdcentraler(Besöktyp typ, Vårdcentral nuvarande, DateTime dag)
    {
        foreach (Vårdcentral annan in  _vårdcentraler)
        {
            if (annan.Id == nuvarande.Id) continue; // Hoppa över den vi redan kollat

            List<LedigaTider> lediga = _tjänst.SökLedigaTider(typ, annan, dag);
            if (lediga.Count == 0) continue;
            
            //Varje persons lista är sorterad, så första tiden är den tidigast.
            DateTime tidigast = lediga.Min(l => l.Tider[0].Start);
            
            Console.WriteLine($" Det finns lediga tider hos {annan.Namn} , tidigast kl. {tidigast:HH:mm}."+
                "Gå tillbabaka och välja den vårdcentralen för att boka där.");
        }
                  
    }
        
    //--------Visa och avboka------
    private void VisaBokningar()
    {
        Patient? patient = HittaEllerRegistrera();
        if (patient == null) return;

        // Tidigast först
        List<Bokning> mina = _tjänst.HämtaBokningar(patient.Personnummer)
                                    .OrderBy(b => b.Tid.Start)
                                    .ToList();
         
            
        
        KonsolHjälp.SkrivBokningar(mina, false,"Du har inga bokningar.");
        
    }
       
        
    private void Avboka()
    {
        Patient? patient = HittaBefintligPatient();
        
        if (patient == null) return;
        
        //Bara aktiva bokninar kan avbokas(inte redan avbokade eller intällda)
         List<Bokning> aktiva= _tjänst.HämtaBokningar(patient.Personnummer)
                                      .Where(b=> b.ÄrAktiv)
                                      .OrderBy(b=> b.Tid.Start)
                                      .ToList();
                
        Console.WriteLine("\n Vilken bokning vill du avboka?");

        Bokning? vald = KonsolHjälp.VäljBokning(aktiva, false);

        if(vald == null) return;      

            //Notisen "Din bokning är avbokad" Skrives av observer.  
        if (!_tjänst.Avboka(vald.Id))
        {
             Console.WriteLine("  Bokningen gick inte att avboka.");
        }
    }

    private Patient? HittaBefintligPatient()
    {
        string? personnummer =KonsolHjälp.LäsPersonnummer("Ditt personnummer(10 sifror, Enter = avbryt): ");

        if (personnummer == null) return null;
        
        Patient? patient =_tjänst.HämtaPatient(personnummer);

        if (patient == null)
        {
            Console.WriteLine(" Vi hittar ingen patient med det personnumret. " +
                                "Boka en tid först för att registrera dig.");
        }
        return patient;
    } 

     private void VisaNotiser()
    {
        Patient? patient = HittaBefintligPatient(); //frågar om personnummer
        if (patient == null) return;

        List<string> notiser = _tjänst.HämtaNotiser(patient.Personnummer);
        Console.WriteLine();
        if (notiser.Count == 0)
        {
            Console.WriteLine("Du har inga notiser."); return;
        }
        foreach(string notis in notiser)
        {
            Console.WriteLine("-"+ notis);
        }
    } 

}



            
 