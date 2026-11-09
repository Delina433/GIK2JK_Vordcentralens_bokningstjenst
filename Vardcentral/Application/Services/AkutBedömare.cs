using Vardcentral.Domain;
using Vardcentral.Domain.Enums;

namespace Vardcentral.Application.Services;

public class AkutBedömare
{
    //ja på någon av dessa ->RÖD
    public List<string> RödaFrågor{get;} = new List<string>
    {
        "Är patienten medvetslös eller har mycket svårt att andas?",
        "Har patienten kraftig bröstsmärta eller kraftig blödning?",
    };
    //Jag på någon av dessa -> Gul
    public List<string> GulaFrågor{get;} = new List<string>
    {
        "Har patienten hög feber eller svår smärta som behöver ses i dag?",
        "Har patienten en skada som behöver ses i dag (t.ex. djupt sår)?",
    };

    public BokningsPrioritet Bedöm(List<bool>rödaSvar, List<bool> gulaSvar)
    {
        if(rödaSvar.Contains(true)) return BokningsPrioritet.Röd;
        if(gulaSvar.Contains(true)) return BokningsPrioritet.Gul;
        return BokningsPrioritet.Grön;
        

    }
}