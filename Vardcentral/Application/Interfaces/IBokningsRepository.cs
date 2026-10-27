using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IBokningsRepository
{
     public void Boka (Bokning bokning);
     public void showBokning(int id);
     public void ShowBokings();
     public void UpdateBoking(Bokning bokning);
}
