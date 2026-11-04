
using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
namespace Vardcentral.Application.Interfaces;

public interface IBokningsService
{
    Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient);

    Bokning? BokaAkut(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag, Vårdcentral vårdcentral);

    bool AvbokaBokning(int bokningId);

    bool StällInBokning(Bokning bokning);

    List<Bokning> HämtaBokningar(string personanummer);

    List<LedigaTider> SökledigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag);

    List<Bokning> HämtaAktivaSorterade();
}
