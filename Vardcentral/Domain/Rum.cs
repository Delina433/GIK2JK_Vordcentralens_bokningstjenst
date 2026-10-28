namespace Vardcentral.Domain;

public class Rum
{         // Id och Namn bara läsas,inte ändras
        public int Id { get; }
        public string Namn { get; }
        // konstruktorn körs när ett nytt rum skapas
        public Rum(int id, string namn)
        {
            //Sparar värdena som kommer in
            Id = id;
            Namn = namn;
        }

    }


    
