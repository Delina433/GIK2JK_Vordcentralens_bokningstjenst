namespace Vardcentral.Domain;

public class LedigaTider
{
    public Personal Personal {get;}
    public List<Tidslucka> Tider {get;}

    public LedigaTider(Personal personal, List<Tidslucka> tider)
    {
        Personal = personal;
        Tider = tider;
    }
}