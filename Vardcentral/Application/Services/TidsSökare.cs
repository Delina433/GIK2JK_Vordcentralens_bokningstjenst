using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
using Vardcentral.Domain.Enums;

namespace Vardcentral.Application.Services;
/// Letar lediga tidergenom att kontrollera personalens arbetstider,
/// besökstyper, vårdcentraler och befintliga bokningar.
public class TidsSökare : ITidsSökare
{
    // Konstanter som anger vårdcentralens öppettider och tidsintervallet
    // mellan varje möjlig starttid.
    private const int Öppnar = 8;
    private const int Stänger = 17;
    private const int StegMinut = 5;
    
    // Repository som används för att hämta personal.
    private readonly IPersonalRepository _personal;

    // Repository som används för att hämta befintliga bokningar.
    private readonly IBokningsRepository _bokningar;
    
   /// Skapar en TidsSökare och tar emot de repositories
   /// som behövs för att söka efter lediga tider.
    public TidsSökare(IPersonalRepository personal, IBokningsRepository bokningar)
    {
        _personal = personal;
        _bokningar = bokningar;
    }
    
    /// Söker efter lediga tider för en viss besökstyp,
    /// vårdcentral och dag.
    public List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag)
    {
      // Lista som kommer att innehålla personal och deras lediga tider.
       List<LedigaTider> resultat = new List<LedigaTider>();
       
       // Stängt på helger.
       if(dag.DayOfWeek == DayOfWeek.Saturday || dag.DayOfWeek == DayOfWeek.Sunday)
       {
        return resultat;
       }
       
       //Hämtar alla personal
       List<Personal> allPersonal = _personal.HämtaAlla();
        
        // Går igenom varje person i listan.
       foreach (Personal personal in allPersonal)
        {
            //kontrolerar vilken typ av personal som behövs
            if(personal.Typ != typ.KrävdPersonal) continue;
                        
            // Kontrollerar att personalen tillhör den valda vårdcentralen.
            if (personal.Vårdcentral.Id != vårdcentral.Id) continue;
            
            // Lista som ska innehålla den aktuella personalens lediga tider.
            List<Tidslucka> tider = new List<Tidslucka>();

            // Anger när arbetsdagen börjar och slutar.
            DateTime start = dag.Date.AddHours(Öppnar); //
            DateTime stägning = dag.Date.AddHours(Stänger);//
        
         //Skapa möjliga tider under arbetsdagen
            while (start.AddMinutes(typ.LängdMinut) <= stägning)
            { 
              // Skapar en tidslucka med starttid och sluttid.
                // Besökets längd hämtas från den valda besökstypen.
                Tidslucka tid= new Tidslucka(start, start.AddMinutes(typ.LängdMinut));

                 // Kontrollerar att tiden ligger i framtiden.
                if (tid.Start> DateTime.Now)
                {
                    // Kontrollerar att personalen och rummet inte redan är bokade.
                    if (ÄrTillgänglig(personal, personal.Rum, tid))
                    {
                        // Lägger till tidsluckan om den är ledig.
                        tider.Add(tid);
                    }
            
                }
            
                 //Flyttar fram starttiden fem minuter för att undersöka nästa möjliga tid.
                start = start.AddMinutes(StegMinut);

            }

            //Lägg till personal om det finns lediga tider
            if (tider.Count > 0)
            {
                resultat.Add(new LedigaTider(personal, tider));
            }
    
        }
       // Returnerar alla personal som har lediga tider och deras tidsluckor.
        return resultat;
        
    }
    /// Kontrollerar om en viss personal och ett visst rum
    /// är tillgängliga under den angivna tidsluckan.
    public bool ÄrTillgänglig(Personal personal, Rum rum, Tidslucka tid)
    {
        // Hämtar alla bokningar från bokningsrepositoryt.    
        List<Bokning> bokningar = _bokningar.HämtaAllaBokningar();
       
        // Går igenom samtliga bokningar.
        foreach (Bokning bokning in bokningar)
        {           
        // Kontrollerar endast bokningar som fortfarande är aktiva.
            if (bokning.ÄrAktiv)
            {
                // Kontrollerar om den befintliga bokningen överlappar
                // med tidsluckan som vi försöker boka.
                if (bokning.Tid.Överlappar(tid))
                {  
                  // Om antingen personalen eller rummet redan är bokat
                    // under samma tidsperiod är tiden inte tillgänglig.
                    if (bokning.Personal.Id == personal.Id || bokning.Rum.Id == rum.Id)
                    {
                        return false;
                    }
                }
            }  
            
        }
         // Om ingen bokning hindrar tiden från att användas är den tillgänglig.
        return true;
    }
}
    
