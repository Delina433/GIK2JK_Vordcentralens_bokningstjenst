using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
namespace Vardcentral.Application.Strategier;
// sorterar bokningar efter tid .Tidigaste tiden kommer först.
public class TidsordningsRegeln : IPrioritetStrategi

{
    public int Jämför(Bokning a, Bokning b)
    {
        //  Tidigast tid först.
        return a.Tid.Start.CompareTo(b.Tid.Start);
    }
}
