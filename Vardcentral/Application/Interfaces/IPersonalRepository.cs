using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IPersonalRepository
{
   // Hämtar en vårdpersonal med id
      Personal? Hämta(string id); // Returerar null om id inte finns 
      // Hämtar all vårdpersonal
      List<Personal> HämtaAlla();
  }
  

