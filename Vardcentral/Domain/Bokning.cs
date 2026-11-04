using Vardcentral.Domain.Enums;

namespace Vardcentral.Domain;
// En bokning: vem, hos vem, vilken typ, när, i vilket rum och status.
// private set: Status, Tid, Personal och Rum ändras bara via metoderna nedan


public class Bokning
{
    public int Id {get;}
    public Patient Patient {get;}
    public Besöktyp Typ {get;}
    public BokningsPrioritet Prioritet {get;}

    //Dessa kan ändras vi ombokning men bara inifrån klassen 
    public Personal Personal {get; private set;}
    public Tidslucka Tid {get; private set;} 
    public Rum Rum {get; private set;}
    public BokningsStatus Status {get; private set;}
    
     
   // En bokning är aktiv om en är bokad eller ombokad (inte avkodad eller inställd)
    public bool ÄrAktiv
    {
        get {return Status == BokningsStatus.Bokad || Status == BokningsStatus.Ombokad; }
    }

    public Bokning(int id, Patient patient, Personal personal, Besöktyp typ, Tidslucka tid, BokningsPrioritet prioritet)
    {
        Id = id;
        Patient = patient;
        Personal = personal;
        Typ = typ;
        Tid = tid;
        Rum = personal.Rum; // besöket sker i personalens rum
        Prioritet = prioritet;
        Status = BokningsStatus.Bokad;
    }
  // Pasienter avbokar själva
    public void Avboka()
    {
        Status = BokningsStatus.Avbokad;
    }

// Vårdcentralen ställerin och ingen ny tid hittades 
    public void StällIn()
    {
        Status = BokningsStatus.Inställd;
    }

//Bokning flyttas till en annan personal och  eller annan tid.
    public void Omboka(Personal nyPersonal, Tidslucka nyTid)
    {
        Personal = nyPersonal;
        Tid = nyTid;
        Rum = nyPersonal.Rum;
        Status = BokningsStatus.Ombokad;
    }
    

}

