using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IBokningsObserver
{
    //Den anropas när bokning ändringen publiceras
    //Den tar emot bokningen och meddelandet som skickas från NotifieringsService
    void Uppdatera(Bokning bokning, string meddelande);
}