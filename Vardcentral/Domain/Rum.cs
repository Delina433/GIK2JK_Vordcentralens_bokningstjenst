namespace Vardcentral.Domain;

// Ett behandlingsrum.
// Kundmötet: ett rum kan delas av flera i personalen,
// men ett rum kan inte ha två bokningar samtidigt det kontrolleras i TidsSökare.



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


    
