namespace Vardcentral.Domain.Enums;
   public class Personal
   {
       public string Namn { get; }
       public string Efternamn { get; }
       public Personaltyp Typ { get; }
       public Vårdcentral Vårdcentral { get; }
       public Rum Rum { get; }
       private string _id = "";
    


       public string Id

       {
           get { return _id; }
           private set
           {

               if (value == null || value.Length != 6 || !value.All(char.IsDigit))
                   throw new ArgumentException("Id måste vara 6 siffror");

               _id = value;
           }
       }

       public Personal(string namn, string efternamn, Personaltyp typ, string id, Vårdcentral vårdcentral, Rum rum)
       {
           Namn = namn;
           Efternamn = efternamn;
           Typ = typ;
           Id = id;
           Vårdcentral = vårdcentral;
           Rum = rum;
       }
    }

