namespace Vardcentral.UI;

public class KonsolHjälp
{
    private const int Bredd = 58;

    public static void Rubrik(string text)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', Bredd));
        Console.WriteLine("" + text);
        Console.WriteLine(new string('=', Bredd));
    }

    public static void KörMeny(string rubrik, string avslutaText, params MenyVal[] val)
    {
       while(true)
        {
            Rubrik(rubrik);
             for (int i = 0; i < val.Length; i ++)
            {
                Console.WriteLine($"{i+1}. {val[i].Text}");
            }
            Console.WriteLine($"0. {avslutaText}");

            int nr = LäsHeltal("\nVal: ", 0, val.Length);
            if (nr == 0) 
            return; // 0 = lämna menyn

            try
            {
                val[nr -1]. Gör(); // kör det valda alternativet
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Det gick inte : {ex.Message}");
            }
            
        } 
    }
}