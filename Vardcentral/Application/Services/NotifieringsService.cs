using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;


namespace Vardcentral.Application.Services;
// Håller en lista med observatörer och skickar meddelanden till alla.

public class NotifieringsService : INotifieringsService
{
    private readonly List<IBokningsObserver> _observatörer = new List<IBokningsObserver>();
    
    private readonly Dictionary<string, List<string>> _sparadeNotiser = new Dictionary<string, List<string>>();
    public void Prenumerera(IBokningsObserver observer)
    {
        if (!_observatörer.Contains(observer))// samma observatör läggs inte till två gånger
        {
            _observatörer.Add(observer);
        }   
        
    }

    public void Avprenumerera(IBokningsObserver observer)
    {
        _observatörer.Remove(observer);
    }


    public void Publicera (Bokning bokning, string meddelande)
    {   
        //Spara notisen så att patienten kan läsa den 
        string pnr = bokning.Patient.Personnummer;
        if(!_sparadeNotiser.ContainsKey(pnr))
        {
            _sparadeNotiser[pnr]= new List<string>();
        }

        _sparadeNotiser[pnr].Add($"{DateTime.Now:yyy-MM-dd HH:mm} {meddelande}");

        //Skickar till alla obsörvatörer
        foreach (IBokningsObserver observer in _observatörer)
        {
            observer.Uppdatera(bokning, meddelande);
        }
    }
    public List<string> HämtaNotiser(string personnumer)
    {
        if(_sparadeNotiser.TryGetValue(personnumer, out List<string>? lista))
        {
            return new List<string>(lista); //Kopia, så ingen utanför kan ändra listan
        }
        return new List<string>();
    }




}
