using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Interfaces;
using System.ComponentModel;

// BokningsService hanterar bokningar och kontaktar de andra klasserna som behövs.

namespace Vardcentral.Application.Services;

public class BokningsService : IBokningsService
{
    //Repository används för att hämta och spara bokningar och personal.
    private readonly IBokningsRepository _bokningar;
    private readonly IPersonalRepository _personal;

    //Används för att hitta lediga tider och skicka meddelanden.
    private readonly ITidsSökare _tidsSökare;
    private readonly INotifieringsService _notiser;
    //Bestämmer vilken prioritet en bokning ska ha.
    private readonly IPrioritetStrategi _prioritet;

    private int _nästaId = 1; // räknare för bokningar

    //hur långt fram systemet letar ny tid vis inställt besök
    private const int SökDagarFramåt = 14; 
    
    //Tar emot de klasser som BokningsService behöver använda.
    public BokningsService(IBokningsRepository bokningar, IPersonalRepository personal,  ITidsSökare tidsSökare, INotifieringsService notiser, IPrioritetStrategi prioritet)
    {
        _bokningar = bokningar;
        _personal = personal;
        _tidsSökare = tidsSökare;
        _notiser = notiser;
        _prioritet = prioritet;
    }
    //GRÖN:Skapar en vanlig bokning med grön prioritet
    public Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient)
    {
        return Skapa (typ, personal, tid, patient, BokningsPrioritet.Grön);
    }
    // Gul: Försöker hitta tidigast lediga tid för en akut bokning.
    public Bokning? BokaAkut(Besöktyp akutTyp, Vårdcentral vårdcentral, DateTime dag, Patient patient)
    {
        //Börjar med att söka ledig tid på vald vårdcentral
        Bokning? bokning = BokaTidigast(akutTyp, vårdcentral, dag, patient);
        if (bokning != null)
        {
            return bokning;
        }
     
        //Om det inte finns i den första letar den i den andra vårdcentralen
        foreach ( Vårdcentral annan in HämtaAndraVårdcentraler(vårdcentral))
        {
            bokning = BokaTidigast(akutTyp, annan, dag, patient);
            if (bokning != null)
            {
                return bokning;
            }


        }
        //Om ingen tid hitas i båda försöker systemet ta över en Grön (normal) bokning.
        bokning = TaÖverGrönTid(akutTyp, vårdcentral, dag, patient);
        if (bokning !=null) return bokning;

        //tar över på någon av de två vårdcentralen
        foreach(Vårdcentral annan in HämtaAndraVårdcentraler(vårdcentral))
        {
            bokning = TaÖverGrönTid(akutTyp, annan, dag, patient);
            if (bokning !=null) return bokning;
        }
        //Returerar null om ingen tid kunde hittas
        return null;
            
        
    }
    //Avboka en bokning med hjälp av boknings ID(Peronnummmer)
    public bool Avboka(int bokningsId)
    {
        //Hämta bokningen från listan 
        Bokning? bokning = _bokningar.HämtaBokning(bokningsId);

        // kontrollera att bokningen finna och forfrande är aktiv 
        if (bokning == null || !bokning.ÄrAktiv)
        {
            return false;
        }
        
        //Ändrar bokningen status till avbokad.
        bokning.Avboka();
        //Sparar ändringen och skickar ett meddelande till patienten.
        _bokningar.UppdaderaBokning(bokning);
        _notiser.Publicera(bokning, "Din bokning är avbokad.");


        return true;
    }
    //Ställer in en bokning och försöker hitta en ny tid åt patienten.
    public bool StällIn(int bokningId)
    {
        //Hämtar bokningen som ska ställas in
        Bokning? bokning =_bokningar.HämtaBokning(bokningId);
        //Kontrollerar att bokningen finns och är aktiv
        if (bokning == null || !bokning.ÄrAktiv)
        {
            return false;
        }
        //Sparar den ursprungliga tiden innan bokninegn ändras
        DateTime gammalTid = bokning.Tid.Start;
        //Börjar med bokningens vårdcentral och lägger till de andra vårcentralen.
        List<Vårdcentral> vårdcentraler = new List<Vårdcentral>{bokning.Personal.Vårdcentral};
        vårdcentraler.AddRange(HämtaAndraVårdcentraler(bokning.Personal.Vårdcentral));

        //Söker efter lediga tider under de kommande 14 dagarna.
        for (int i = 0; i<= SökDagarFramåt; i++)
        {
            DateTime dag = gammalTid.Date.AddDays(i);

            //Går igenom varje vårdcentral
            foreach(Vårdcentral vc in vårdcentraler)
            {
                //Hämta lediga tider för samma typ av besök
                foreach(LedigaTider lediga in _tidsSökare.SökLedigaTider(bokning.Typ, vc, dag))
                {
                    //Hoppar över personalen som hade den ursprungliga bokningen.
                    if(lediga.Personal.Id == bokning.Personal.Id) continue;

                    //Hittar den första lediga tiden som är efter den gamla tiden
                    Tidslucka? nyTid = lediga.Tider.FirstOrDefault(t => t.Start> gammalTid);
                    //Går vidare om det inte finns någon passande tid
                    if(nyTid == null) continue;

                    //Hämtar den personal och bokar om besöket
                    Personal nyPersonal = lediga.Personal;
                    bokning.Omboka(nyPersonal,nyTid);
                    //Sparar den nya bokningen och skickar information till patienten
                    _bokningar.UppdaderaBokning(bokning);
                    _notiser.Publicera(bokning,$"Din tid {gammalTid:yyyy-MM-dd HH:mm} är inställd. Ny tid: {nyTid.Start:yyyy-MM-dd HH:mm} " +
                        $"hos {nyPersonal.Namn} {nyPersonal.Efternamn}, {nyPersonal.Rum.Namn}, {nyPersonal.Vårdcentral.Namn}. " +
                        "Avboka om tiden inte passar.");
                    return true;
                }
            
            }

               
        }

        //Om ingen ny tid hittas ställs bokningen in helt.
        bokning.StällIn();
        //Sparar ändringen och meddelar patienten
        _bokningar.UppdaderaBokning(bokning);
        _notiser.Publicera(bokning, "Din tid är tyvärr inställd. Boka gärna en ny tid.");
        
        return true;
    }
    //Hämta alla bokningar som tillhöe en viss patient
    public List<Bokning> HämtaBokningar(string personnummer)
    {
        //Skapar en lista för patienten bokningar 
        List<Bokning> resultat = new List<Bokning>();

        //Går igenom alla bokningar
        foreach (var bokning in _bokningar.HämtaAllaBokningar())
        {
            //Kontrllerar om bokningen tillhör patientens personnummer.
            if (bokning.Patient.Personnummer ==  personnummer)
            {
                resultat.Add(bokning);
            }
            
        }
        //Returerar alla bokningar som tillhör patienten 
        return resultat;
    }
   //Hämta lediga tider för en viss typ av besök, på en Vårdcentral och dag
   public List<LedigaTider>SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag)
    {
        return _tidsSökare.SökLedigaTider(typ, vårdcentral, dag);
    }

    //Hämtar alla aktiva bokningar och sorterar dem efter prioritet
    public List<Bokning> HämtaAktivaSorterade()
    {
        //Skapar en lista för aktiva bokningar
        List<Bokning> aktiva = new List<Bokning>();

        //Går igenom alla bokningar
        foreach (var bokning in _bokningar.HämtaAllaBokningar())
        {
            //Läger till bokningen om den fortfarande är aktiv
            if (bokning.ÄrAktiv)
            {
                aktiva.Add(bokning);
            }
            
        }
        //Sorterar bokningar enligt de regler som finns i prioritetstrategin
        aktiva.Sort(_prioritet.Jämför);
        return aktiva;
    }
    //Letar efter den tidigaste lediga tiden på en vårdcentral
    private Bokning? BokaTidigast(Besöktyp akutTyp, Vårdcentral vårdcentral, DateTime dag, Patient patient)
    {
        //Används för att spara den tidigaste lediga tiden
        LedigaTider?  bästa = null;
        //Går igenom alla lediga tider som hittas
        foreach (LedigaTider lediga in _tidsSökare.SökLedigaTider(akutTyp, vårdcentral, dag))
        {   
            //Hoppar över personal som inte har någon ledig tid
            if (lediga.Tider.Count == 0) continue;
            //Jämför tiderna för att hitta den tidigaste
            if (bästa == null || lediga.Tider[0].Start <bästa.Tider[0].Start)
            {
                bästa = lediga;
            }
        }
        if (bästa ==null) //null om ingen ledig tid hittas
          return null;

        //Sparar en bokning med gul prioritet på den tidigasre lediga tiden
        return Skapa(akutTyp, bästa.Personal, bästa.Tider[0], patient, BokningsPrioritet.Gul); 

    }
    //Försöker hitta en grön bokning som kan lämna plats åt akutbesök.
    private Bokning? TaÖverGrönTid(Besöktyp akutTyp, Vårdcentral vårdcentral, DateTime dag, Patient patient)
    {
        //Används  för att spara den gröna bokningen som ska tas över
        Bokning? grön = null;

        foreach (Bokning b in _bokningar.HämtaAllaBokningar())
        {   //Hoppar över oaktiva bokningar
            if (!b.ÄrAktiv) continue;
            //Ändast gröna bokningar kan tas över av akut
            if(b.Prioritet != BokningsPrioritet.Grön) continue; // Gul tar aldrig över gul
            //Kontrolerar att peronalen har rättt kompetens
            if(b.Personal.Typ != akutTyp.KrävdPersonal) continue;
            //Kontrallerar att bokningen tillhör rätt vårdcentral 
            if (b.Personal.Vårdcentral.Id != vårdcentral.Id) continue;
            //Kontrolerar att boknigen gäller rätt dag och ligger i framtiden
            if(b.Tid.Start.Date !=dag.Date || b.Tid.Start <= DateTime.Now) continue;
            //sparar den tidigaste gröna bokningen
            if (grön == null || b.Tid.Start < grön.Tid.Start)
            {
                grön = b;
            }
        }
        if (grön == null) return null;
        //Ställer in den gröna bokninegn så att tiden blir ledig
        grön.StällIn();
        _bokningar.UppdaderaBokning(grön);
        _notiser.Publicera(grön, $"Din tid {grön.Tid.Start:yyyy-MM-dd HH:mm} har fått ge plats åt ett akut besök. Boka ärna en ny tid.");

        //Gul tar över just den gröna tidsluckan
        return Skapa(akutTyp, grön.Personal, grön.Tid, patient, BokningsPrioritet.Gul);
    }
    //Hämtar alla vårdcentraler förutom den som redan är vald
    private List<Vårdcentral>HämtaAndraVårdcentraler(Vårdcentral nuvarande)
    {   //Lista över de andra vårdcentalerna
        List<Vårdcentral> andra = new List<Vårdcentral>();

        foreach (var person in _personal.HämtaAlla())
        {   //Hoppar över personal som tillhör den nuvarande vårdcentralen
            if (person.Vårdcentral.Id == nuvarande.Id) continue;
            
            bool finnsRedan = false;
            //Kontrolerar om vårcentralen redan finns i listan
            foreach (var vc in andra)
            {
                if (vc.Id == person.Vårdcentral.Id)
                {
                    finnsRedan = true;
                } 
               
            }
            //Lägger till vårdcentral om den inte redan finns
            if (!finnsRedan)
            {
                andra.Add(person.Vårdcentral);
            }          
            
        }
        return andra;
    }
    //Skapa ny bokning om personalen och tiden är tillgängliga
    private Bokning? Skapa(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient, BokningsPrioritet prioritet)
    {   //Kontrolerar att personalen har rätt typ för besöket
        if (personal.Typ != typ.KrävdPersonal)
        {
            return null; // t.ex sjuksköterska kan inte bokas hos en läkare
        }
 
        //Kontrollerar att personalen och rummen är lediga vid den valda tiden 
        if (!_tidsSökare.ÄrTillgänglig(personal, personal.Rum, tid))
        {
            return null; //dubbelbokning hindras
        }
        
        //Skapar en ny bokning med det akuella bokningsnummret
        Bokning bokning = new Bokning(_nästaId, patient, personal, typ, tid, prioritet);
        
        _nästaId++;//ökar bokningsnumer inför nästa bokning
        _bokningar.LäggTillBokning(bokning);
        _notiser.Publicera(bokning, $"Bokad: {typ.Namn} hos {personal.Namn} {personal.Efternamn}, {tid.Start:yyyy-MM-dd HH:mm}, {personal.Rum.Namn}, {personal.Vårdcentral.Namn}.");
        
        return bokning;
    }
}
