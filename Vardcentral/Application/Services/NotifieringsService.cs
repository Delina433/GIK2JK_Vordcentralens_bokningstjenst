using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;


namespace Vardcentral.Application.Services;

public class NotifieringsService : INotifieringsService
{
    private readonly List<IBokningsObserver> _observatörer = new List<IBokningsObserver>();

    public void Prenumerera(IBokningsObserver observer)
    {
        if (!_observatörer.Contains(observer))
        {
            _observatörer.Add(observer);
        }   
        
    }

    public void Avprenumerera(IBokningsObserver observer)
    {
        _observatörer.Remove(observer);
    }

    public void Publicera (Bokning bokning, string meddelande)
    {
        foreach (IBokningsObserver observer in _observatörer)
        {
            observer.Uppdatera(bokning, meddelande);
        }
    }


}