using Vardcentral.Domain;

namespace Vardcentral.Application. Interfaces;

//  Repository: Hämtar personal
public interface IPersonalRepository
{
    // null om id inte finns
    Personal? Hämta(string id); // Returerar null om id inte finns 
 // Hämtar all personal.     
    List<Personal> HämtaAlla();
}
