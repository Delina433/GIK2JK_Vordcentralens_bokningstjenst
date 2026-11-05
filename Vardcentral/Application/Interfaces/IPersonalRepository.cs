using Vardcentral.Domain;

namespace Vardcentral.Application. Interfaces;

// Hämtar personal
public interface IPersonalRepository
{
    // Hämtar en vårdpersonal med id
    Personal? Hämta(string id); // Returerar null om id inte finns 
 // Hämtar all personal.     
    List<Personal> HämtaAlla();
}
