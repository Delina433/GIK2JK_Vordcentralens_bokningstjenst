using Vardcentral.Domain;


namespace Vardcentral.Application.Interfaces;

// ALLT en patient kan göra. PatientMeny partar bara med det här interfacet
public interface IPatientService
{
    // Hämtarpatient med personnummer. Ger null om den inte finns.
    Patient? HämtaPatient(string personnummer);
    // Registrerar en ny patient och sparar den.
    Patient RegistreraPatient(string förnamn, string efternamn, string personnummer);

    List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime  dag);
 //Ger null om tiden inte  är ledig eller om det är fel sorts personal.
    Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient);

    // Ger False om bokningen inte finns eller redan är inaktiv.
    bool Avboka(int bokningId);

    List<Bokning> HämtaBokningar(string personnummer);

}