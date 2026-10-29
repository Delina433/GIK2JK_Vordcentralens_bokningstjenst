using Vardcentral.Domain;


namespace Vardcentral.Application.Interfaces;

public interface ITidsSökare
{
    //Söker efter lediga tider för en viss typ av vårdbesök.
    //Returerar en lista med lediga tidsluckor för varje vårdpersonal.
    List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag);
     
     //Kontrolerar om vårdpersonal och rum är lediga under den angivna tidsluckan.
     //Returerar true om tiden är ledig
    bool ÄrTillgänglig(Personal personal, Rum rum, Tidslucka tid);
}