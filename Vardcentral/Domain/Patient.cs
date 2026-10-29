namespace Vardcentral.Domain;

public class Pacient
{
    public string Namn { get; set; }
    public string Efternamn { get; set; }
    private string _Personnummer;

    public string Personnummer

    { get { return _Personnummer; } set { 
        if (value == null || value.Length != 10 || !value.All(char.IsDigit))
            { throw new ArgumentException("Personnummer måste vara 10 siffror");
                _Personnummer = value;
            }
        } }

    public Pacient(string namn, string efternamn, string personnummer)
{
    Namn = namn;
    Efternamn = efternamn;
    Personnummer = personnummer;

}
}
 List<Pacient> Pacienter = new List<Pacient>();
