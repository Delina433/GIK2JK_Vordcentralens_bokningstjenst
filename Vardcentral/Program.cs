using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Interfaces;
using Vardcentral.Application.Services;
using Vardcentral.Application.Strategier;
using Vardcentral.Infrastructure;
using Vardcentral.UI;


// två vårdcentraler
var norr = new Vårdcentral(1, "Vårdcentralen Norr");
var söder = new Vårdcentral(2, "Vårdcentralen Söder");
var Vårdcentraler = new List<Vårdcentral> { norr, söder };
// Rum kan delas av flera 
var rumNorr1 = new Rum(1, "Rum 1 (Norr)");
var rumNorr2 = new Rum(2, "Rum 2(Norr)");
var rumSöder1 = new Rum(3, "Rum 1 ( Söder)");
var rumSöder2 = new Rum(4, "Rum 2 ( Söder)");

var personalRepo = new MinnePersonalRepository();
personalRepo.LäggTill(new Personal("Ahmad", "Anna", Personaltyp.Läkare, "100001", norr, rumNorr1));
personalRepo.LäggTill(new Personal("Sarra", "Diana", Personaltyp.Sjuksköterska, "100002", norr, rumNorr2));
personalRepo.LäggTill(new Personal("Adem", "Rena", Personaltyp.Fysioterapeut, "100003", norr, rumNorr2));
personalRepo.LäggTill(new Personal("Fanar", "Delina", Personaltyp.Läkare, "200001", söder, rumSöder1));
personalRepo.LäggTill(new Personal("Eva", "Fredik", Personaltyp.Sjuksköterska, "200002", söder, rumSöder2));
personalRepo.LäggTill(new Personal("Nour", "Aya", Personaltyp.Fysioterapeut, "200003", söder, rumSöder2));

// Våra antagnaden:fasta längder.
var besökstyper = new List<Besöktyp>
{
    new Besöktyp("Läkarbesök", 10, Personaltyp.Läkare),
    new Besöktyp("sjuksköterskebesök", 20, Personaltyp.Sjuksköterska),
    new Besöktyp("Fysioterapi", 45, Personaltyp.Fysioterapeut),
};
var akutTyp = new Besöktyp("Akut", 30, Personaltyp.Sjuksköterska);// personalen avgör akut

// Påhittade patienter.
var patienteRepo = new MinnePatientRepository();
patienteRepo.LäggTill(new Patient("Anna", "Ahmad", "1111111111"));
patienteRepo.LäggTill(new Patient("Diana", "Sarra", "2222222222"));
patienteRepo.LäggTill(new Patient("Rena", "Adem", "3333333333"));

// koppla ihop klasserna//
var bokningRepo = new MinneBokningRepository();
var tidsSökare = new TidsSökare(personalRepo, bokningRepo);
var notiser = new NotifieringsService();
notiser.Prenumerera(new KonsolNotifierare());// observer 
IBokningsService service = new BokningsService(bokningRepo, personalRepo, tidsSökare, notiser, new AkutFörstRegel());// Strategy

//---meny---//
IPatientService patientService = new PatientService(service, patienteRepo);
var patientMeny = new PatientMeny(patientService, besökstyper, Vårdcentraler);
var personalMeny = new PersonalMeny(service, patienteRepo, akutTyp);
var huvudMeny = new HuvudMeny(Vårdcentraler, patientMeny, personalMeny);
huvudMeny.Kör();

