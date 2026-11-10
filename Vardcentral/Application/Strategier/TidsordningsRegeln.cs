using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
namespace Vardcentral.Application.Strategier;
// Sratgey:bara tid, tidigast tid först.
public class TidsordningsRegeln : IPrioritetStrategi

{
    public int Jämför(Bokning a, Bokning b)
    {
        //  Tidigast tid först.
        return a.Tid.Start.CompareTo(b.Tid.Start);
    }
}
