namespace Vardcentral.Domain;

public class Vårdcentral 
{
    public int Id {get;}
    public string Namn {get;}

    public Vårdcentral (int id, string namn)
    {
        Id = id;
        Namn = namn;
    }
}