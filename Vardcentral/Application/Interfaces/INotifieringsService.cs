using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;
// Skickar ut notiser när en bokning ändras. (Observer mönster)
public interface INotifieringsService
{
    //börja få notiser.
    void Prenumerera(IBokningsObserver observer);

    //sluta få notiser.
    void Avprenumerera(IBokningsObserver observer);

    //skicka notis till alla.
    void Publicera(Bokning bokning, string meddelande);

    //en patient sparade notiser. Hämtar alla sparade notiser för en patient.
    List<string> HämtaNotiser(string personnummer);
}
