using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;

namespace Vardcentral.Infrastructure;


public class MinnePersonalRepository : IPersonalRepository
{
    private readonly List<Personal> _personal = new List<Personal>();

    public void LäggTill(Personal person)
    {
        _personal.Add(person);
    }

    public Personal? Hämta(string id)
    {
        foreach (var person in _personal)
        {
            if (person.Id == id)
                return person;
        }
        return null;
    }
    public List<Personal> HämtaAlla()
    {
        return new List<Personal>(_personal);

    }



}
