using Vardcentral.Application.Interfaces;
using Vardcentral.Domain;

namespace Vardcentral.Application.Services;
/// Hanterar patientrelaterade operationer i systemet.
/// Använder bokningsservice, patientrepository och notifieringsservice
/// för att hämta och registrera patienter, hantera bokningar
/// och hämta patienternas notiser.
public class PatientService : IPatientService
{    /// Ansvarar för att utföra bokningsrelaterade operationer.
    private readonly IBokningsService _bokningar;
    
    /// Ansvarar för att hämta och spara patienter.
    private readonly IPatientRepository _patienter;
    
  /// Ansvarar för att hämta patienternas sparade notiser.
    private readonly INotifieringsService _notiser;

    /// Skapar en PatientService och tar emot de beroenden
    /// som klassen behöver för att utföra sina uppgifter.
    public PatientService(IBokningsService bokningar, IPatientRepository patienter,INotifieringsService notiser) 
    {   
       /// Sparar de beroenden som skickas in via konstruktorn.
        _bokningar = bokningar;
        _patienter = patienter;
        _notiser = notiser;
    }
    /// Hämtar en patient utifrån personnummer
    public Patient? HämtaPatient(string personnummer)
    {   
     // Skickar vidare sökningen till patientrepositoryt.
        return _patienter.Hämta(personnummer);
    }

        /// Skapar och registrerar en ny patient.
    public Patient RegistreraPatient(string förnamn, string efternamn, string personnummer)
    {
        Patient ny = new Patient(förnamn, efternamn, personnummer);
        _patienter.LäggTill(ny);
        return ny;
    }

    /// Söker efter lediga tider för en viss besökstyp,
    /// vårdcentral och dag.
    public List<LedigaTider> SökLedigaTider(Besöktyp typ, Vårdcentral vårdcentral, DateTime dag)
    {
    /// skickar sökningen till bokningsservicen.
        return _bokningar.SökLedigaTider(typ, vårdcentral, dag);
    }
    
    /// Försöker skapa en bokning för en patient.
    public Bokning? SkapaBokning(Besöktyp typ, Personal personal, Tidslucka tid, Patient patient)
    { 
        /// Skickar vidare informationen till bokningsservicen,
       /// som ansvarar för själva bokningshanteringen.
        return _bokningar.SkapaBokning(typ, personal, tid,patient);
    }
    /// Försöker avboka en bokning utifrån dess ID.
    public bool Avboka(int bokningId)
    { 
      /// Överlåter avbokningen till bokningsservicen.
        return _bokningar.Avboka(bokningId);
    }
    
    /// Hämtar alla bokningar som tillhör en viss patient.
    public List<Bokning> HämtaBokningar(string personnummer)
    {  
       /// Hämtar patientens bokningar via bokningsservicen.
        return _bokningar.HämtaBokningar(personnummer);
    }

    /// Hämtar alla sparade notiser för en viss patient.
    public List<string> HämtaNotiser(string personnummer)
    { 
      /// Hämtar patientens notiser från notifieringsservicen.
        return _notiser.HämtaNotiser(personnummer);
    }
}
 
