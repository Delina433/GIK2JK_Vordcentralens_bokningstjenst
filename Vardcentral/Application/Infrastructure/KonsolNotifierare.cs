using Vardcentral.Domain;
using Vardcentral.Application.Interfaces;



public class KonsolNotifierare : IBokningsObserver
{
    public void Uppdatera(Bokning bokning, string meddelande)
    {
        Console.WriteLine($"[Notis till {bokning.Patient.Namn} {bokning.Patient.Efternamn}] {meddelande}");

    }

}
