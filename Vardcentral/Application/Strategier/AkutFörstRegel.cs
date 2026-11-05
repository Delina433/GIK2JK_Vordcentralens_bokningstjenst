using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Interfaces;

namespace Vardcentral.Application.Strategier;
//Akuta bokningar först, sedan tidigaste tiden (strategy mönster)
public class AkutFörstRegel : IPrioritetStrategi

{
    public int Jämför(Bokning a, Bokning b)
    {
        // olika prioritet : den högsta prioriteten först 
        if (a.Prioritet != b.Prioritet)
        {
            return a.Prioritet.CompareTo(b.Prioritet);
        }
        // samma prioritet: tidigast tid först.
        return a.Tid.Start.CompareTo(b.Tid.Start);
    }
}



