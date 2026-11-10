using Vardcentral.Domain;


namespace Vardcentral.Application.Interfaces;
// Hittar lediga tider och kontrollerar om en tid är ledig
public interface ITidsSökare
{
    //Lediga tider per personal för en besökstyp på en viss dag. 
    
    List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag);
     
     //True om båda personal och rum är lediga under hela tidsluckan. False annars.
    bool ÄrTillgänglig(Personal personal, Rum rum, Tidslucka tid);
}
