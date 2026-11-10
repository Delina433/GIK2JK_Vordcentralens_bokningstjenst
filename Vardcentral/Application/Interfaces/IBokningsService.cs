
using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
namespace Vardcentral.Application.Interfaces;
// Det menyn (Program) får anropa.Bokningsservice är encontroller.

public interface IBokningsService
{
    //  Grön:Vanlig bokning. Ger null om tiden inte är ledig eller om det är fel  personal.
    Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient);

    // Gul: tidigast lediga tid i dag på vald vårdcentral. Ger null om ingen tid finns.
    Bokning? BokaAkut(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag, Patient patient);
    
     // Patienten avbokar. Ger false om bokningen inte finns eller redan är inaktiv.
    bool Avboka(int bokningId);
    
// Personalen ställer in. patienten får en tid om det finns.
    bool StällIn(int bokning);
    //Alla bokningar för en patient.

    List<Bokning> HämtaBokningar(string personanummer);
    //Lediga tider för en viss typ av besök på en viss vårdcentral på en viss dag.

    List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag);
    
 // Aktiva bokningar sorterade med vald regel. 
    List<Bokning> HämtaAktivaSorterade();
}
