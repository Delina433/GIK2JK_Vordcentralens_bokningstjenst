using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

// Repository: sparar och hämtar patienter. 

public interface IPatientRepository
{ 
    void LäggTill(Patient patient);//Sparar patient
    Patient? Hämta(string personnummer);// null om patienten inte finns.
}