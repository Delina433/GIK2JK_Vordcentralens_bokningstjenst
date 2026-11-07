using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;



namespace Vardcentral.Infrastructure;


public class MinneBokningRepository : IBokningsRepository
{
    private readonly Dictionary<int, Bokning> _bokningar = new();

    public void LäggTillBokning(Bokning bokning)
    {
        if (bokning == null)
        throw new ArgumentNullException(nameof(bokning));
    
        _bokningar.Add(bokning.Id, bokning);
    }

    //Ger null om id:t inte finns.
    public Bokning? HämtaBokning(int id)
    {
        return _bokningar.TryGetValue(id, out Bokning? bokning) ? bokning : null;
    }

    //Returerar en kopia som lista
    public List<Bokning> HämtaAllaBokningar()
    {
        return new List<Bokning>(_bokningar.Values);

    }

    public void UppdaderaBokning(Bokning bokning)
    {
        if(bokning == null)
        throw new ArgumentNullException(nameof(bokning));
        
        if (!_bokningar.ContainsKey(bokning.Id))
        throw new ArgumentException("Bokningen finns inte.");

        _bokningar[bokning.Id] = bokning;
    }
}
