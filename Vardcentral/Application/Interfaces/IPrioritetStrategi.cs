using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;
// Strategy: en regel för vilken bokning som kommer först
public interface IPrioritetStrategi
{
            // Negativt tal om a kommer före b, positivt tal om b kommer före a, 0 om de är lika.
            int Jämför(Bokning a, Bokning b);
        }
    

