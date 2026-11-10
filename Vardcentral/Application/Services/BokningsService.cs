using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
// Tar emot anrop från menyn och styr de andra klasserna.

namespace Vardcentral.Application.Services;

public class BokningsService : IBokningsService
{
    private readonly IBokningsRepository _bokningar;
    private readonly IPersonalRepository _personal;
    private readonly ITidsSökare _tidsSökare;
    private readonly INotifieringsService _notiser;
    private readonly IPrioritetStrategi _prioritet;

    private int _nästaId = 1; // räknare för bokningar
    //hur långt fram systemet letar ny tid vis instäldbesök
    private const int SökDagarFramåt = 14; 

    public BokningsService(IBokningsRepository bokningar, IPersonalRepository personal,  ITidsSökare tidsSökare, INotifieringsService notiser, IPrioritetStrategi prioritet)
    {
        _bokningar = bokningar;
        _personal = personal;
        _tidsSökare = tidsSökare;
        _notiser = notiser;
        _prioritet = prioritet;
    }
    //Grön: Vanlig bokning
    public Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient)
    {
        return Skapa (typ, personal, tid, patient, BokningsPrioritet.Grön);
    }
    // Gul: tidigast lediga tid i dag på vald vårdcentral.
    public Bokning? BokaAkut(Besöktyp akutTyp, Vårdcentral vårdcentral, DateTime dag, Patient patient)
    {
        //Ledig tid på vald vårdcentral
        Bokning? bokning = BokaTidigast(akutTyp, vårdcentral, dag, patient);
        if (bokning != null)
        {
            return bokning;
        }
     
        //Ledig tid på den andra vårdcentralen
        foreach ( Vårdcentral annan in HämtaAndraVårdcentraler(vårdcentral))
        {
            bokning = BokaTidigast(akutTyp, annan, dag, patient);
            if (bokning != null)
            {
                return bokning;
            }


        }
        
        bokning = TaÖverGrönTid(akutTyp, vårdcentral, dag, patient);
        if (bokning !=null) return bokning;

        foreach(Vårdcentral annan in HämtaAndraVårdcentraler(vårdcentral))
        {
            bokning = TaÖverGrönTid(akutTyp, annan, dag, patient);
            if (bokning !=null) return bokning;
        }

        return null;
            
        
    }

    public bool Avboka(int bokningsId)
    {
        Bokning? bokning = _bokningar.HämtaBokning(bokningsId);

        if (bokning == null || !bokning.ÄrAktiv)
        {
            return false;
        }
        

        bokning.Avboka();
        _bokningar.UppdaderaBokning(bokning);
        _notiser.Publicera(bokning, "Din bokning är avbokad.");


        return true;
    }

    public bool StällIn(int bokningId)
    {
        Bokning? bokning =_bokningar.HämtaBokning(bokningId);
        
        if (bokning == null || !bokning.ÄrAktiv)
        {
            return false;
        }

        DateTime gammalTid = bokning.Tid.Start;
        
        List<Vårdcentral> vårdcentraler = new List<Vårdcentral>{bokning.Personal.Vårdcentral};
        vårdcentraler.AddRange(HämtaAndraVårdcentraler(bokning.Personal.Vårdcentral));

        
        for (int i = 0; i<= SökDagarFramåt; i++)
        {
            DateTime dag = gammalTid.Date.AddDays(i);
                
            foreach(Vårdcentral vc in vårdcentraler)
            {
                foreach(LedigaTider lediga in _tidsSökare.SökLedigaTider(bokning.Typ, vc, dag))
                {
                    //Inte samma personal (t.ex. om hen är sjuk)
                    if(lediga.Personal.Id == bokning.Personal.Id) continue;

                    //Förstq lediga tid efter den inställda tiden
                    Tidslucka? nyTid = lediga.Tider.FirstOrDefault(t => t.Start> gammalTid);
                    if(nyTid == null) continue;

                    Personal nyPersonal = lediga.Personal;
                    bokning.Omboka(nyPersonal,nyTid);
                    _bokningar.UppdaderaBokning(bokning);
                    _notiser.Publicera(bokning,$"Din tid {gammalTid:yyyy-MM-dd HH:mm} är inställd. Ny tid: {nyTid.Start:yyyy-MM-dd HH:mm} " +
                        $"hos {nyPersonal.Namn} {nyPersonal.Efternamn}, {nyPersonal.Rum.Namn}, {nyPersonal.Vårdcentral.Namn}. " +
                        "Avboka om tiden inte passar.");
                    return true;
                }
            
            }

               
        }

        //Ingen ledig tid inom 14 dagar
        bokning.StällIn();
        _bokningar.UppdaderaBokning(bokning);
        _notiser.Publicera(bokning, "Din tid är tyvärr inställd. Boka gärna en ny tid.");
        
        return true;
    }
    public List<Bokning> HämtaBokningar(string personnummer)
    {
        List<Bokning> resultat = new List<Bokning>();

        foreach (var bokning in _bokningar.HämtaAllaBokningar())
        {
            if (bokning.Patient.Personnummer ==  personnummer)
            {
                resultat.Add(bokning);
            }
            
        }
        return resultat;
    }
   
   public List<LedigaTider>SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag)
    {
        return _tidsSökare.SökLedigaTider(typ, vårdcentral, dag);
    }

    //Sortera med de valda reglerna
    public List<Bokning> HämtaAktivaSorterade()
    {
        List<Bokning> aktiva = new List<Bokning>();

        foreach (var bokning in _bokningar.HämtaAllaBokningar())
        {
            if (bokning.ÄrAktiv)
            {
                aktiva.Add(bokning);
            }
            
        }
        aktiva.Sort(_prioritet.Jämför);
        return aktiva;
    }

    private Bokning? BokaTidigast(Besöktyp akutTyp, Vårdcentral vårdcentral, DateTime dag, Patient patient)
    {
        LedigaTider?  bästa = null;

        foreach (LedigaTider lediga in _tidsSökare.SökLedigaTider(akutTyp, vårdcentral, dag))
        {
            if (lediga.Tider.Count == 0) continue;
            if (bästa == null || lediga.Tider[0].Start <bästa.Tider[0].Start)
            {
                bästa = lediga;
            }
        }
        if (bästa ==null)
          return null;

        return Skapa(akutTyp, bästa.Personal, bästa.Tider[0], patient, BokningsPrioritet.Gul); 

    }
    private Bokning? TaÖverGrönTid(Besöktyp akutTyp, Vårdcentral vårdcentral, DateTime dag, Patient patient)
    {
        Bokning? grön = null;

        foreach (Bokning b in _bokningar.HämtaAllaBokningar())
        {
            if (!b.ÄrAktiv) continue;
            if(b.Prioritet != BokningsPrioritet.Grön) continue; // Gul tar aldrig över gul
            if(b.Personal.Typ != akutTyp.KrävdPersonal) continue; 
            if (b.Personal.Vårdcentral.Id != vårdcentral.Id) continue;
            if(b.Tid.Start.Date !=dag.Date || b.Tid.Start <= DateTime.Now) continue;

            if (grön == null || b.Tid.Start < grön.Tid.Start)
            {
                grön = b;
            }
        }
        if (grön == null) return null;
        //Släpp den gröna tiden först, annars säger TidsSökare att tiden är upptagen.
        grön.StällIn();
        _bokningar.UppdaderaBokning(grön);
        _notiser.Publicera(grön, $"Din tid {grön.Tid.Start:yyyy-MM-dd HH:mm} har fått ge plats åt ett akut besök. Boka ärna en ny tid.");

        //Gul tar över just den gröna tidsluckan
        return Skapa(akutTyp, grön.Personal, grön.Tid, patient, BokningsPrioritet.Gul);
    }

    private List<Vårdcentral>HämtaAndraVårdcentraler(Vårdcentral nuvarande)
    {
        List<Vårdcentral> andra = new List<Vårdcentral>();

        foreach (var person in _personal.HämtaAlla())
        {
            if (person.Vårdcentral.Id == nuvarande.Id) continue;
            
            bool finnsRedan = false;
            foreach (var vc in andra)
            {
                if (vc.Id == person.Vårdcentral.Id)
                {
                    finnsRedan = true;
                } 
               
            }    
            if (!finnsRedan)
            {
                andra.Add(person.Vårdcentral);
            }          
            
        }
        return andra;
    }

    private Bokning? Skapa(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient, BokningsPrioritet prioritet)
    {  
        if (personal.Typ != typ.KrävdPersonal)
        {
            return null; // t.ex sjuksköterska kan inte bokas hos en läkare
        }
 

        if (!_tidsSökare.ÄrTillgänglig(personal, personal.Rum, tid))
        {
            return null; //dubbelbokning hindras
        }
        

        Bokning bokning = new Bokning(_nästaId, patient, personal, typ, tid, prioritet);

        _nästaId++;
        _bokningar.LäggTillBokning(bokning);
        _notiser.Publicera(bokning, $"Bokad: {typ.Namn} hos {personal.Namn} {personal.Efternamn}, {tid.Start:yyyy-MM-dd HH:mm}, {personal.Rum.Namn}, {personal.Vårdcentral.Namn}.");
        
        return bokning;
    }
}
