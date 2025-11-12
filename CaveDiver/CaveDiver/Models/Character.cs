using CaveDiver.Engine;
using CaveDiver.Dialogue;

namespace CaveDiver.Models;

public abstract class Character
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Strength { get; set; }
    public int Defense { get; set; }
    public int Intelligence { get; set; }
    public bool IsAlive => Health > 0;
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public List<Item> Inventory { get; set; } = new List<Item>();
    public const int MaxInventorySize = 5;


    public Character(string name, int health, int maxHealth, int strength, int defense, int intelligence)
    {
        Name = name; 
        Health = health; 
        MaxHealth = maxHealth; 
        Strength = strength; 
        Defense = defense;
        Intelligence = intelligence;
        Inventory = new List<Item>();
    }

    public virtual int Attack()
    {
        var roll = Dice.Roll(8) / 2;
        var damageDealt = Strength + roll;
        
        GameUtils.TypeLine($"{Name} rolled {roll}, {Name} dealt {damageDealt} damage.");
        return Strength + roll;
    }

    public virtual void TakeDamage(int damage)
    {
        int damageTaken = Math.Max(0, damage - Defense);
        Health -= damageTaken;
        GameUtils.TypeLine($"{Name} suffered {damageTaken} damage points, {Health}/{MaxHealth} HP remains");
    }

    public virtual void GainExperience(int amount)
    {
        Experience += amount;
        GameUtils.TypeLine($"{Name} has gained {amount} XP!");

        int nextLevel = Level * 100;
    }

    public bool AddItem(Item item)
    {
        if (Inventory.Count >= MaxInventorySize)
        {
            GameUtils.TypeLine($"{Name} cannot carry anymore items");
            return false;
        }

        Inventory.Add(item);
        ApplyItemStats(item);
        GameUtils.TypeLine($"{Name} picked up {item.Name}");
        return true;
    }

    public void ApplyItemStats(Item item)
    {
        Strength += item.StrengthBonus;
        Defense += item.DefenseBonus;
        Intelligence += item.IntelligenceBonus;
    }

    public void RemoveItem(Item item)
    {
        if (!Inventory.Contains(item))
        {
            GameUtils.TypeLine($"{Name} does not hold this {item.Name}");
            return;
        }
        Inventory.Remove(item);
        Strength -= item.StrengthBonus;
        Defense -= item.DefenseBonus;
        Intelligence -= item.IntelligenceBonus;

        GameUtils.TypeLine($"{Name} unequipped {item.Name}.");
    }

    public void SwapItem(Item newItem, Item oldItem)
    {
        RemoveItem(oldItem);
        AddItem(newItem);
    }
}
