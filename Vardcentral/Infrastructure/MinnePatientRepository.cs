using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace Vardcentral.Infrastructure;

// sparar patienter i minnet.Dictionery: personnummer--> patient
public class MinnePatientRepository : IPatientRepository
{      
    private readonly Dictionary<string, Patient> _Patienter = new Dictionary<string, Patient>();
    // Sparar en patient
    public void LäggTill(Patient patient)
    {
        _Patienter[patient.Personnummer] = patient;

    }
    // Hämtar patienten , eller null om den inte finns
    public Patient? Hämta(string personnummer)
    {

        if (_Patienter.TryGetValue(personnummer, out Patient? patient))
            return patient;
        return null;
    }
}



