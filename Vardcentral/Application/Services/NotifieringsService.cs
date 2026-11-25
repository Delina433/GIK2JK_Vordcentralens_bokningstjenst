using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
namespace Vardcentral.Application.Services;

/// Hanterar prenumerationer från observatörer och publicerar
/// meddelanden när en bokningshändelse inträffar.
/// Sparar även patienternas notiser så att de kan hämtas senare.

public class NotifieringsService : INotifieringsService
{
    /// Lista över alla observatörer som prenumererar på bokningshändelser.
    private readonly List<IBokningsObserver> _observatörer = new List<IBokningsObserver>();
    
    /// Dictionary där patientens personnummer används som nyckel
    /// och patientens sparade notiser lagras i en lista.
    private readonly Dictionary<string, List<string>> _sparadeNotiser = new Dictionary<string, List<string>>();

     /// Lägger till en observatör som ska få meddelanden
    /// när en bokningshändelse publiceras.
    public void Prenumerera(IBokningsObserver observer)
    {  
   /// Kontrollerar att samma observatör inte redan finns i listan.
   if (!_observatörer.Contains(observer)) 
        {
            _observatörer.Add(observer);
        }   
        
    }
    
    /// Tar bort en observatör från prenumerationslistan.
    /// Observatören får då inte längre några publicerade meddelanden.
    public void Avprenumerera(IBokningsObserver observer)
    
    {   // Remove tar bort observatören från listan om den finns.
        _observatörer.Remove(observer);
    }

    /// Publicerar ett meddelande om en bokningshändelse.
    /// Sparar först notisen för patienten och skickar sedan
    /// meddelandet till samtliga prenumererande observatörer.
    public void Publicera (Bokning bokning, string meddelande)
    {   
         /// Kontrollerar om patienten redan har en lista med sparade notiser.
        if(!_sparadeNotiser.ContainsKey(pnr))
        
        {    /// Skapar en ny tom lista om patienten inte har några sparade notiser.
            _sparadeNotiser[pnr]= new List<string>();
        }
        
         /// Sparar meddelandet tillsammans med datum och tid.
        _sparadeNotiser[pnr].Add($"{DateTime.Now:yyy-MM-dd HH:mm} {meddelande}");

        /// Går igenom alla observatörer som har prenumererat.
        foreach (IBokningsObserver observer in _observatörer)
        {                 
            /// Meddelar varje observatör om bokningshändelsen.
            observer.Uppdatera(bokning, meddelande);
        }
    }

     /// Hämtar alla sparade notiser för en viss patient.
    /// Returnerar en kopia av listan så att den ursprungliga
    /// listan inte kan ändras utifrån.
    public List<string> HämtaNotiser(string personnumer)
    {  
    /// Försöker hämta patientens notiser med personnumret som nyckel.
        if(_sparadeNotiser.TryGetValue(personnumer, out List<string>? lista))
        {
         /// Returnerar en kopia så att originalet inte kan ändras utifrån.
            return new List<string>(lista); ///Kopia, så ingen utanför kan ändra listan
        }
        /// Om patienten inte finns i Dictionary returneras en tom lista.
        return new List<string>();
    }




}
