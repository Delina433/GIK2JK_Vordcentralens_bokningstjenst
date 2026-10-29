namespace Vardcentral.Domain;
   public class Personal
   {
       public string Namn { get; set; }
       public string Efternamn { get; set; }
       public string Personaltyp { get; set; }

       private string _id;
       public string Id

       {
           get { return _id; }
           set
           {

               if (value == null || value.Length != 6 || !value.All(char.IsDigit))


               {
                   throw new ArgumentException("Id måste vara 6 siffror");
               }

               _id = value;
           }
       }

       public Personal(string namn, string efternamn, string personaltyp, string id)
       {
           Namn = namn;
           Efternamn = efternamn;
           Personaltyp = personaltyp;
           Id = id;

       }

       List<Personal> vardpersonal = new List<Personal>();
   }
