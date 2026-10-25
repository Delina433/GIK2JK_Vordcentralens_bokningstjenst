namespace Vardcentral.Domain;

public class Rum
{         // Id och Namn bara läsas,inte ändras
        public int Id { get; }
        public string Name { get; }
        // konstruktorn körs när ett nytt rum skapas
        public Rum(int id, string namn)
        {
            //Sparar värdena som kommer in
            Id = id;
            Namn = namn;
        }

    }
}

    
