using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IBokningsRepository
{
      void LäggTillBokning (Bokning bokning);
     Bokning?  HämtaBokning(int id); // Null om id inte finns
     List<Bokning> HämtaAllaBokningar();
     void UppdaderaBokning(Bokning bokning);
}
