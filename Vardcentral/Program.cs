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
var rumNorr2 = new Rum(2, "Rum 2 (Norr)");
var rumNorr3 = new Rum(2, "Rum 3 (Norr)");
var rumNorr4 = new Rum(2, "Rum 4 (Norr)");
var rumNorr5 = new Rum(2, "Rum 5 (Norr)");
var rumSöder1 = new Rum(3, "Rum 1 ( Söder)");
var rumSöder2 = new Rum(4, "Rum 2 ( Söder)");
var rumSöder3 = new Rum(4, "Rum 3 ( Söder)");
var rumSöder4 = new Rum(4, "Rum 4 ( Söder)");
var rumSöder5 = new Rum(4, "Rum 5 ( Söder)");



var personalRepo = new MinnePersonalRepository();
//Vårdcentral
personalRepo.LäggTill(new Personal("Ahmad", "Ali", Personaltyp.Läkare, "100001", norr, rumNorr1));
personalRepo.LäggTill(new Personal("Anna", "Andersson", Personaltyp.Läkare, "100002", norr, rumNorr2));
personalRepo.LäggTill(new Personal("Sarra", "Berg", Personaltyp.Sjuksköterska, "100003", norr, rumNorr3));
personalRepo.LäggTill(new Personal("Diana", "Karlsson", Personaltyp.Sjuksköterska, "100004", norr, rumNorr4));
personalRepo.LäggTill(new Personal("Adem", "Johansson", Personaltyp.Fysioterapeut, "100005", norr, rumNorr5));
personalRepo.LäggTill(new Personal("Rena", "Larsson", Personaltyp.Fysioterapeut, "100006", norr, rumNorr5));

//Vårdcentral söder
personalRepo.LäggTill(new Personal("Karin", "Olsson", Personaltyp.Läkare, "200007", söder, rumSöder1));
personalRepo.LäggTill(new Personal("Lena", "Lassis", Personaltyp.Läkare, "200008", söder, rumSöder2));
personalRepo.LäggTill(new Personal("Eva", "Svensson", Personaltyp.Sjuksköterska, "200009", söder, rumSöder3));
personalRepo.LäggTill(new Personal("Fredrik", "Persson", Personaltyp.Sjuksköterska, "200010", söder, rumSöder4));
personalRepo.LäggTill(new Personal("Nour", "Gustafsson", Personaltyp.Fysioterapeut, "200011", söder, rumSöder5));
personalRepo.LäggTill(new Personal("Aya", "Skog", Personaltyp.Fysioterapeut, "200012", söder, rumSöder5));

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
patienteRepo.LäggTill(new Patient("Elin", "Isaksson", "1111111111"));
patienteRepo.LäggTill(new Patient("Kalle", "Persson", "2222222222"));
patienteRepo.LäggTill(new Patient("Rin", "Brun", "3333333333"));

// koppla ihop klasserna//
var bokningRepo = new MinneBokningRepository();
var tidsSökare = new TidsSökare(personalRepo, bokningRepo);
var notiser = new NotifieringsService();
notiser.Prenumerera(new KonsolNotifierare());// observer 
IBokningsService service = new BokningsService(bokningRepo, personalRepo, tidsSökare, notiser, new AkutFörstRegel());// Strategy

//---meny---//
IPatientService patientService = new PatientService(service, patienteRepo, notiser);
var patientMeny = new PatientMeny(patientService, besökstyper, Vårdcentraler);
var personalMeny = new PersonalMeny(service, patienteRepo, akutTyp, new AkutBedömare());
var huvudMeny = new HuvudMeny(Vårdcentraler, patientMeny, personalMeny);
huvudMeny.Kör();

