using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IBokningsService
{
    Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient);

    
}