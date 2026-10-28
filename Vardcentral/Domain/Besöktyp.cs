using Vardcentral.Domain.Enums;

namespace Vardcentral.Domain;

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




    
