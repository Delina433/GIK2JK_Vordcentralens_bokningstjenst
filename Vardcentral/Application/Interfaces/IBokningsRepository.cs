using Vardcentral.Domain;

namespace Vardcentral.Application.Interfaces;

public interface IBokningsRepository
{
     public void Boka (Bokning bokning);
     public void VisaBokning(int id);
     public void VisaBokningar();
     public void UpdaderaBokning(Bokning bokning);
}
