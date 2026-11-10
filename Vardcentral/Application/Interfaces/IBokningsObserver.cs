using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;
//  Observer: den som vill få notiser när en bokning ändras. (Observer mönster)
public interface IBokningsObserver
{
    // Anropas av NotifieringsService när en bokning ändras.
    //  Den skickar meddelandet vidare till alla som prenumererar på notiser.
    void Uppdatera(Bokning bokning, string meddelande);
}
