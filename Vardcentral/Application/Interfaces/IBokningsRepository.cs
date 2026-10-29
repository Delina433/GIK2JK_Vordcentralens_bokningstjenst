using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IBokningsRepository
{
      void LäggTillBokning (Bokning bokning);
     Bokning?  HämtaBokning(int id);
     List<Bokning> HämtaAllaBokningar();
     void UppdaderaBokning(Bokning bokning);
}
