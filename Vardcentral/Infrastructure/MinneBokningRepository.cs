using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;



namespace Vardcentral.Infrastructure;

// Sparar bokningar i minnet. Dictionery: id-->bokning
public class MinneBokningRepository : IBokningsRepository
{
    private readonly Dictionary<int, Bokning> _bokningar = new();
// sparar en ny bokning.
    public void LäggTillBokning(Bokning bokning)
    {
        if (bokning == null)
        throw new ArgumentNullException(nameof(bokning));
    
        _bokningar.Add(bokning.Id, bokning);
    }

    //  Hämtar bokningen, eller null om id inte finns.
    public Bokning? HämtaBokning(int id)
    {
        return _bokningar.TryGetValue(id, out Bokning? bokning) ? bokning : null;
    }

    //kopia som lista , så ingen utanför kan ändrar   vår samling.
    public List<Bokning> HämtaAllaBokningar()
    {
        return new List<Bokning>(_bokningar.Values);

    }
// Sparar en ändrad bokning.fel om den inte finns.
    public void UppdaderaBokning(Bokning bokning)
    {
        if(bokning == null)
        throw new ArgumentNullException(nameof(bokning));
        
        if (!_bokningar.ContainsKey(bokning.Id))
        throw new ArgumentException("Bokningen finns inte.");

        _bokningar[bokning.Id] = bokning;
    }
}
