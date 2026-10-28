using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface INotifieringsService
{
    //Registrerar en obseervatör som ska får info 
    void Prenumerera(IBokningsObserver observer);

    //Tar bort en observatör så att den inte får info
    void Avprenumerera(IBokningsObserver observer);

    //Publicerar ett meddelande när något ändras
    void Publicera(Bokning bokning, string meddelande);
}