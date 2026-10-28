namespace Vardcentral.Domain;

public class LedigaTider
{
    public Vårdpersonal Personal {get;}
    public List<Tidslucka> Tider {get;}

    public LedigaTider(Vårdpersonal personal, List<Tidslucka> tider)
    {
        Personal = personal;
        Tider = tider;
    }
}