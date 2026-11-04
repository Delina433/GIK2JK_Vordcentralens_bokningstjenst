using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;



public interface IPatientRepository
{
    void LäggTill(Patient patient);
    Patient? Hämta(string personnummer);
}