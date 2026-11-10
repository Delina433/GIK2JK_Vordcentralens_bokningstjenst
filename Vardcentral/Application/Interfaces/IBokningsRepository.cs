using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;
// Observer: den som vill få notiser när en bokning ändras. (Observer mönster)
public interface IBokningsRepository
{       void LäggTillBokning (Bokning bokning);
     Bokning?  HämtaBokning(int id); // Null om id inte finns
     List<Bokning> HämtaAllaBokningar();
     // Anropas av NotifieringsService när en bokning ändras. Den skickar meddelandet vidare till alla som prenumererar på notiser.
     void UppdaderaBokning(Bokning bokning);
}
