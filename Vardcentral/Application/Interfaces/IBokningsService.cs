
using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
namespace Vardcentral.Application.Interfaces;
// Det menyn (Program) får anropa

public interface IBokningsService
{
    // Vanlig bokning. Ger null om tiden inte är ledig eller om det är fel sorts personal.
    Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient);

    // Akut bokning (görs av personal). Tar första lediga tiden. Ger null om inget finns.
    Bokning? BokaAkut(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag, Patient patient);
    
     // Patienten avbokar. Ger false om bokningen inte finns eller redan är inaktiv.
    bool Avboka(int bokningId);
    
// Vårdcentralen ställer in 
    bool StällIn(int bokning);

    List<Bokning> HämtaBokningar(string personanummer);

    List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag);
    
 // Alla aktiva bokningar sorterade med vald prioritetsregel.
    List<Bokning> HämtaAktivaSorterade();
}
