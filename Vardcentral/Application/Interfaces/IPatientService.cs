using Vardcentral.Domain;


namespace Vardcentral.Application.Interfaces;

// ALLT en patient får göra. PatientMeny prtar bara med det här interfacet
public interface IPatientService
{
    
    Patient? HämtaPatient(string personnummer);// null om den inte finns
    
    Patient RegistreraPatient(string förnamn, string efternamn, string personnummer);

    List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime  dag);
 //Ger null om tiden inte  är ledig eller om det är fel  personal.
    Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient);

    // Ger False om bokningen inte finns eller redan är inaktiv.
    bool Avboka(int bokningId);

    List<Bokning> HämtaBokningar(string personnummer);
    List<string> HämtaNotiser(string personnummer);

}