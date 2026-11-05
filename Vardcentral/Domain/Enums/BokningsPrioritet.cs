namespace Vardcentral.Domain.Enums;
// Normal eller Akut. Kundmötet: det är personalen (sjuksköterskan) som avgör akut, inte patienten.

public enum  BokningsPrioritet
{
    Röd, // livshotande ambulansen skickas direkt

    Gul, // akut: tidigaste lediga tid i dag, anars hänvisas till den andra Vårdcentralen 

    Grön, //vanliga bokningar 
}
