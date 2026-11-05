using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
using Vardcentral.Domain.Enums;

namespace Vardcentral.Application.Services;
// Letar lediga tider
public class TidsSökare : ITidsSökare
{
    private const int Öppnar = 8;
    private const int Stänger = 17;
    private const int StegMinut = 5;

    private readonly IPersonalRepository _personal;
    private readonly IBokningsRepository _bokningar;

    public TidsSökare(IPersonalRepository personal, IBokningsRepository bokningar)
    {
        _personal = personal;
        _bokningar = bokningar;
    }

    public List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag)
    {
       List<LedigaTider> resultat = new List<LedigaTider>();
       // Stängt på helger.
       if(dag.DayOfWeek == DayOfWeek.Saturday || dag.DayOfWeek == DayOfWeek.Sunday)
       {
        return resultat;
       }
       
       //Hämtar alla personal
       List<Personal> allPersonal = _personal.HämtaAlla();

       foreach (Personal personal in allPersonal)
        {
            //kontrolerar vilken typ av personal som behövs
            if(personal.Typ != typ.KrävdPersonal) continue;
            if (personal.Vårdcentral.Id != vårdcentral.Id) continue;

            List<Tidslucka> tider = new List<Tidslucka>();

            DateTime start = dag.Date.AddHours(Öppnar); //
            DateTime stägning = dag.Date.AddHours(Stänger);//
        
         //Skapa möjliga tider under arbetsdagen
            while (start.AddMinutes(typ.LängdMinut) <= stägning)
            {
                Tidslucka tid= new Tidslucka(start, start.AddMinutes(typ.LängdMinut));

                if (tid.Start> DateTime.Now)
                {
                    if (ÄrTillgänglig(personal, personal.Rum, tid))
                    {
                        tider.Add(tid);
                    }
            
                }
            
                 //
                start = start.AddMinutes(StegMinut);

            }

            //Lägg till personal om det finns lediga tider
            if (tider.Count > 0)
            {
                resultat.Add(new LedigaTider(personal, tider));
            }
    
        }
        return resultat;
        
    }
    public bool ÄrTillgänglig(Personal personal, Rum rum, Tidslucka tid)
    {
        List<Bokning> bokningar = _bokningar.HämtaAllaBokningar();

        foreach (Bokning bokning in bokningar)
        {
            if (bokning.ÄrAktiv)
            {
                if (bokning.Tid.Överlappar(tid))
                {
                    if (bokning.Personal.Id == personal.Id || bokning.Rum.Id == rum.Id)
                    {
                        return false;
                    }
                }
            }  
            
        }
        return true;
    }
}
    
