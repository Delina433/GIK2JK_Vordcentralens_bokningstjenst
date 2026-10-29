using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IPersonalRepository
{
   // Hämtar en vårdpersonal med id
      Vårdpersonal? Hämta(int id);
      // Hämtar all vårdpersonal
      List<Vårdpersonal> HämtaAlla();
  }
  
}
