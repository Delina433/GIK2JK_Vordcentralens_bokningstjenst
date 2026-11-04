using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
using System.Data.Common;


namespace Vardcentral.Infrastructure;


public class MinneBokningRepository : IBokningsRepository
{
    private readonly List<Bokning> _bokningar = new();

    public void LäggTillBokning(Bokning bokning)
    {
        if (bokning == null)
        throw new ArgumentNullException(nameof(bokning));
    
        _bokningar.Add(bokning);
    }

    public Bokning? HämtaBokning(int id)
    {
        foreach (var bokning in _bokningar)
        {
            if (bokning.Id == id)
                return bokning;
        }
        return null;
    }
    public List<Bokning> HämtaAllaBokningar()
    {
        return new List<Bokning>(_bokningar);

    }
    public void UppdaderaBokning(Bokning bokning)
    {
        if(bokning == null)
        throw new ArgumentNullException(nameof(bokning));
        
        for (int i = 0; i <_bokningar.Count; i++)
        {
            if (_bokningar[i].Id == bokning.Id)
            {
                _bokningar[i] = bokning;
                return;
            }
        }

        throw new ArgumentException("Bokningen finns inte.");
    }
}
