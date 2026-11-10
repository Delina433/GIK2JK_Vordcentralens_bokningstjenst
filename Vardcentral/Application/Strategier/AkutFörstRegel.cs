using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Interfaces;

namespace Vardcentral.Application.Strategier;
//strategy:Röd Först, sedan gul ,sedan grön. Om samma prioritet, tidigast tid först.
public class AkutFörstRegel : IPrioritetStrategi

{
    public int Jämför(Bokning a, Bokning b)
    {
        // olika  färg :lägst värde först. Röd=0, Gul=1, Grön=2
        if (a.Prioritet != b.Prioritet)
        {
            return a.Prioritet.CompareTo(b.Prioritet);
        }
        // samma färg: tidigast tiden först.
        return a.Tid.Start.CompareTo(b.Tid.Start);
    }
}



