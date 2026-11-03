using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;
namespace Vardcentral.Application.Infrastructure;
     // sparar bokningar i minnet i en list( ingen databas -avgränsningar)
public class MinnesBokningRepository : IBokningsRepository
{
    private readonly List<Bokning> _bokningar = new List<Bokning>();

     // lägger till en bokning sist i listan.
    public void LäggTillBokning(Bokning bokning)
    {
        _bokningar.Add(bokning);
    }   // linjär sökning: går ignom listan tills rätt id hittas.
    public Bokning? HämtaBokning(int id)
    {
        foreach (var bokning in _bokningar)
        {
            if (bokning.Id == id)
                return bokning;
        }
        return null;
    }  // Ger en kopia av listan ( inkapsling)
    public List<Bokning> HämtaAllaBokningar()
    {
        return new List<Bokning>(_bokningar);
    }  // letar upp bokningen med samma id  och byter ut den.
    public void UppdateraBokning(Bokning bokning)
    {
        for (int i = 0; i < _bokningar.Count; i++)
        {
            for (int i = 0; i < _bokningar.Count; i++)
            {
                if (_bokningar[i].Id == bokning.Id)
                {
                    _bokningar[i] = bokning;
                    return;
                }
            }
        }

    }
}