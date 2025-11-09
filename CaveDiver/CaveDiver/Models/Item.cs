namespace CaveDiver.Models;

public class Item
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int StrengthBonus { get; set; }
    public int DefenseBonus { get; set; }
    public int IntelligenceBonus { get; set; }

    public Item(string name, string type, int str = 0, int def = 0, int intl = 0)
    {
        Name = name; 
        Type = type; 
        StrengthBonus = str; 
        DefenseBonus = def; 
        IntelligenceBonus = intl;
    }

    public override string ToString()
    {
        string bonuses = "";
        if (StrengthBonus != 0)
        {
            bonuses += $"+{StrengthBonus} STR";
        }
        if (DefenseBonus != 0)
        {
            bonuses += $"+{DefenseBonus} STR";
        }
        if (IntelligenceBonus != 0)
        {
            bonuses += $"+{IntelligenceBonus} STR";
        }

        return $"{Name} ({Type} {bonuses})";
    }
}
