namespace Vardcentral.Domain;

public class Patient
{
    public string Namn { get; set; }
    public string Efternamn { get; set; }
    private string _Personnummer = "";

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
    
    
    
    
    
    
    
    