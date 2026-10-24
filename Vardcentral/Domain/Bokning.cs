namespace Vardcentral.Domain;

public class Bokning
{
    public int Id {get;}
    public Patient Patient {get;}
    public Vårdpersonal Personal {get; set;} // Det kan ändras vi ombokning
    public Besöktyp Typ {get;}
    public Tidslucka Tid {get; set;} // Kan också ändras vid ombokning
    public BokningsStatus Status {get; set;}
    public BokningsPrioritet Prioritet {get;}
     

    public bool ÄrAktiv
    {
        get {return Status == BokningsStatus.Bokas || Status == BokningsStatus.Ombokad; }
    }

    public Bokning(int id, Patient patient, Vårdpersonal personal, Besöktyp typ, Tidslucka tid, BokningsPrioritet prioritet)
    {
        Id = id;
        Patient = patient;
        Personal = personal;
        Typ = typ;
        Tid = tid;
        Prioritet = prioritet;
        Status = BokningsStatus.Bokad;
    }

}

