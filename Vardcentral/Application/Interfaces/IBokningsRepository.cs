using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;
// Sparar och hämtar bokningar bara lagring.
public interface IBokningsRepository
{
      void LäggTillBokning (Bokning bokning);
     Bokning?  HämtaBokning(int id); // Null om id inte finns
     List<Bokning> HämtaAllaBokningar();
     void UppdaderaBokning(Bokning bokning);
}
