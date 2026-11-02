using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Interfaces;

namespace Vardcentral.Application.Strategier;
//Akuta bokningar först, sedan tidigaste tiden (strategy mönster)
public class AkutFörstRegel : IPrioritetRegel

{
    public int Jämför(Bokning a, Bokning b)
    {
        // olika prioritet : den akuta går först.
        if (a.Prioritet != b.Prioritet)
        {
            if (a.Prioritet == BokningsPrioritet.Akut)
                return -1; // a först
            else
                return 1;//b först
        }
        // samma prioritet: tidigast tid först.
        return a.Tid.Start.CompareTo(b.Tid.Start);
    }
}



