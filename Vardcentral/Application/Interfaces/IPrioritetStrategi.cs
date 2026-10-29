using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IPrioritetStrategi
{
            // Bestämmer vilken patient som ska tas först
            // ex går akut patienter före vanliga
            int Jämför(Bokning a, Bokning b);
        }
    

