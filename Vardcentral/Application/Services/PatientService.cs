using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;

namespace Vardcentral.Application.Services;

public class PatientService : IPatientService
{
    private readonly IBokningsService _bokningar;
    private readonly IPatientRepository _patienter;

    public PatientService(IBokningsService bokningar, IPatientRepository patienter) 
    {
        _bokningar = bokningar;
        _patienter = patienter;
    }

    public Patient? HämtaPatient(string personnummer)
    {
        return _patienter.Hämta(personnummer);
    }
    public Patient RegistreraPatient(string förnamn, string efternamn, string personnummer)
    {
        Patient ny = new Patient(förnamn, efternamn, personnummer);
        _patienter.LäggTill(ny);
        return ny;
    }

    public List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag)
    {
        return _bokningar.SökLedigaTider(typ, vårdcentral, dag);
    }

    public Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient)
    {
        return _bokningar.SkapaBokning(typ, personal, tid,patient);
    }

    public bool Avboka(int bokningId)
    {
        return _bokningar.Avboka(bokningId);
    }

    public List<Bokning> HämtaBokningar(string personnummer)
    {
        return _bokningar.HämtaBokningar(personnummer);
    }
}
 
