namespace Vardcentral.UI;

public class MenyVal
{
    public string Text { get; set; }
    public Action Gör {get; set;}

    public MenyVal (string text, Action gör)
    {
        Text = text;
        Gör = gör;
    }
}