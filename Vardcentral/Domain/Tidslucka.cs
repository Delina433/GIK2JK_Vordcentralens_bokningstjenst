namespace Vardcentral.Domain;

             // en tid med start och slut.
     public class Tidslucka
    {                  //  kan bara läsas,utan att ändras.
        public DateTime Start { get;}
        public DateTime Slut{ get;}

        public Tidslucka(DateTime start, DateTime slut)
        {     
            //Egen kontroll: slut måste vara efter start
            if (slut <= start)
                throw new ArgumentException("Slut måste vara efter start.");

            Start = start;
            Slut = slut;
        }      
             // kollar om två tider krockar, så att två patienter inte bokas samtidigt.
         public  bool Överlappar( Tidslucka annan)
        {
            return Start < annan.Slut && annan.Start < Slut;
        
        }
    }

    
