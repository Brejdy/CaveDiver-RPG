using CaveDiver.Models.Types;

namespace CaveDiver.Models;

public class Enemy : Character
{
    public string Type { get; set; }
    public int ExperienceReward { get; set; }
    public Item? LootItem { get; set; }

    public Enemy(string name, string type) : base("Monster", 0, 0, 0, 0, 0)
    {
        Name = name;
        Type = type;

        switch (Type)
        {
            case EnemyType.Goblin:
                Health = 20;
                MaxHealth = 20;
                Strength = 8;
                Defense = 6;
                ExperienceReward = 100;
                LootItem = new Item("Rusty Dagger", "Weapon", str: 2);
                break;

            case EnemyType.GoblinShaman:
                Health = 25;
                MaxHealth = 25;
                Strength = 10;
                Defense = 8;
                ExperienceReward = 150;
                LootItem = new Item("Magic crystal", "Weapon", intl: 2);
                break;

            case EnemyType.Hobgoblin:
                Health = 45;
                MaxHealth = 45;
                Strength = 30;
                Defense = 25;
                ExperienceReward = 300;
                break;

            case EnemyType.General:
                Health = 100;
                MaxHealth = 100;
                Strength = 80;
                Defense = 70;
                ExperienceReward = 1000;
                break;

            case EnemyType.King:
                Health = 300;
                MaxHealth = 300;
                Strength = 200;
                Defense = 150;
                ExperienceReward = 10000;
                break;
        }
    }
}
