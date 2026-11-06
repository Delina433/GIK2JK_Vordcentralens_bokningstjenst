using Vardcentral.Domain;
namespace Vardcentral.Appliction.Interfaces;

// ALLT en patient kan göra. PatientMeny partar bara med det här interfacet
public interface IPatientRepository
{
    // Hämtarpatient med personnummer. Ger null om den inte finns.
    void LäggTill(Patient patient);
    Patient? Hämta(string personnummer);
    // Registrerar en ny patient och sparar den.
    Patient RegistreraPatient(string förnamn, string efternamn, string personnummer);

    List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime  dag, DateTime dag);
 //Ger null om tiden inte  är ledig eller om det är fel sorts personal.
    Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient, BokningsPrioritet prioritet);

    // Ger False om bokningen inte finns eller redan är inaktiv.
    bool Avboka(int bokningId);

    List<Bokning> HämtaBokningar(string personnummer);

}