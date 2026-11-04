using Vardcentral.Domain.Enums;

using Vardcentral.Application.Interfaces;

namespace Vardcentral.Infrastructure;


public class MinnePersonalRepository : IPersonalRepository
{
    private readonly List<Personal> _personal = new ();

    public void LäggTill(Personal person)
    {
        if (person == null)
            throw new ArgumentNullException(nameof(person));

            
        _personal.Add(person);
    }

    public Personal? Hämta(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

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



