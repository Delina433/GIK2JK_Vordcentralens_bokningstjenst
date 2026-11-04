namespace Vardcentral.Domain;
// En patient
// Personnumret kontrolleras innan det sparas.
// Antagande (kundmötet): inloggning sköts utanför systemet, så vi sparar inget lösenord.

public class Patient
{
   // Bara get: kan läsas men inte ändras utifrån . 
    public string Namn { get; set; }
    public string Efternamn { get; set; }
    private string _Personnummer = "";
   // private set: bara Patient själv kan sätta personnumret, och då kontrolleras det
    public string Personnummer
    {
        get { return _Personnummer; }

        private set 
        { 
            if (value == null || value.Length != 10 || !value.All(char.IsDigit))
            { throw new ArgumentException("Personnummer måste vara 10 siffror"); }

            _Personnummer = value;// sparas bara om kontrollen gick igenom
        }
    }

        public Patient(string namn, string efternamn, string personnummer)
        {
            Namn = namn;
            Efternamn = efternamn;
            Personnummer = personnummer;
        }
    }
    
    
    
    
    
    
    
    
