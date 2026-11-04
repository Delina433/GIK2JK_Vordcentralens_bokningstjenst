using Vardcentral.Domain.Enums;

namespace Vardcentral.Domain;
// En typ av besök. Talar om hur lång tid det tar och vilken sorts personal som krävs.
public class Besöktyp
{  
        public string Namn { get;  }
        public int LängdMinut { get; }
        public Personaltyp KrävdPersonal { get; }

        public Besöktyp(string namn, int längdMinut, Personaltyp krävdPersonal)
        {
            Namn = namn;
            LängdMinut = längdMinut;
            KrävdPersonal = krävdPersonal;
        }
    }




    
