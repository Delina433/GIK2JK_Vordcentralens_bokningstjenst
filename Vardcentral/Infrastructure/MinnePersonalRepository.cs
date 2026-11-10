using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;

namespace Vardcentral.Infrastructure;

//Sparar personal i minnet. List: personal
public class MinnePersonalRepository : IPersonalRepository
{
    private readonly List<Personal> _personal = new List<Personal>();
//lägger till en personal
    public void LäggTill(Personal person)
    {
        _personal.Add(person);
    }
          //letar personal med id eller null om den inte finns
    public Personal? Hämta(string id)
    {
        foreach (var person in _personal)
        {
            if (person.Id == id)
                return person;
        }
        return null;
    }
    // kopia av listan, så ingen utanför kan ändra vår samling.
    public List<Personal> HämtaAlla()
    {
        return new List<Personal>(_personal);

    }

}
