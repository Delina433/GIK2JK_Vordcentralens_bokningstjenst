using Vardcentral.Domain;
using Vardcentral.Domain.Enums;

namespace Vardcentral.Application.Interfaces;

public interface IPersonalRepository
{  //Lägg till vårdpersonal 
      void LäggTill(Personal person);
   // Hämtar en vårdpersonal med id
      Personal? Hämta(string id); // Returerar null om id inte finns 

      // Hämtar all vårdpersonal
      List<Personal> HämtaAlla();
  }
  

