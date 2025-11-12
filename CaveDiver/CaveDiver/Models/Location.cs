using CaveDiver.Dialogue;
using CaveDiver.Models.Types;
using CaveDiver.Engine;

namespace CaveDiver.Models;

public class Location
{
    public string Name { get; set; }
    public string Type { get; set; }
    string Description { get; set; }
    public List<Enemy> Enemies { get; set; } = new List<Enemy>();
    public Merchant? Merchant { get; set; }
    public bool IsCleared { get; set; }
    public List<Location> ConnectedLocations { get; set; } = new List<Location>();
    public Companion? Companion { get; set; }
    public bool HasEnemies => Enemies.Any(e => e.IsAlive);

    public Location(string name, string type, string description)
    {
        Name = name;
        Type = type;
        Description = description;
    }

    public void TryEncounter(Player player, List<Companion> party, GameEngine engine, string phase)
    {
        if (Type == LocationType.Village || Type == LocationType.Town)
            return;

        int roll = Dice.Roll(10);
        if (roll <= 4) 
        {
            GameUtils.TypeLine($"\nWhile {phase}, you are ambushed!");

            List<Enemy> ambushEnemies = new();

            if (Type == LocationType.Forest)
            {
                ambushEnemies.Add(new Enemy("Goblin Scout", EnemyType.Goblin));
                if (Dice.Roll(10) > 5)
                    ambushEnemies.Add(new Enemy("Goblin Shaman", EnemyType.GoblinShaman));
                if (Dice.Roll(10) > 8)
                    ambushEnemies.Add(new Enemy("Bandit", EnemyType.General));
            }
            else if (Type == LocationType.Cave)
            {
                ambushEnemies.Add(new Enemy("Hobgoblin", EnemyType.Hobgoblin));
                if (Dice.Roll(10) > 6)
                    ambushEnemies.Add(new Enemy("Goblin General", EnemyType.General));
                if (Dice.Roll(10) > 9)
                    ambushEnemies.Add(new Enemy("Goblin King", EnemyType.King));
            }

            engine.StartBattle(player, party, ambushEnemies);
        }
        else
        {
            RandomEncounter.Trigger(player, party);
        }
    }

    public void Enter(Player player, List<Companion> party, GameEngine engine)
    {
        GameUtils.TypeLine($"You arrived at {Name}.");
        GameUtils.TypeLine(Description);

        bool exploring = true;

        while (exploring)
        {
            GameUtils.TypeLine("\nWhat would you like to do?");
            GameUtils.TypeLine("1. Look around");
            if (Merchant != null)
            { 
                GameUtils.TypeLine("2. Trade with merchant"); 
            }
            if (Enemies.Any())
            {
                GameUtils.TypeLine("3. Engage in battle");
            }
            if (ConnectedLocations.Any())
            {
                GameUtils.TypeLine("4. Travel to another location");
            }
            GameUtils.TypeLine("5. Rest");
            GameUtils.TypeLine("6. Leave game");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    GameUtils.TypeLine($"You look around. {Description}");
                    break;

                case "2":
                    Merchant?.Trade(player, party);
                    break;

                case "3":
                    if (Enemies.Any())
                    {
                        GameUtils.TypeLine("There are enemies nearby, they have spotted you!");
                        TryEncounter(player, party, engine, "entering the area");
                        engine.StartBattle(player, party, Enemies);
                        Enemies.Clear();

                        if (!player.IsAlive)
                        {
                            exploring = false;
                        }

                        TryMeetCompanion(player, party);
                    }
                    else
                    {
                        GameUtils.TypeLine("There are no enemies left here.");
                        TryMeetCompanion(player, party);
                    }
                    break;

                case "4":
                    if (!ConnectedLocations.Any())
                    {
                        GameUtils.TypeLine("There is nowhere to travel.");
                        break;
                    }

                    GameUtils.TypeLine("Where would you like to travel?");
                    for (int i = 0; i < ConnectedLocations.Count; i++)
                    { 
                        GameUtils.TypeLine($"{i + 1}. {ConnectedLocations[i].Name}"); 
                    }
                    GameUtils.TypeLine($"{ConnectedLocations.Count + 1}. Stay here");

                    if (int.TryParse(Console.ReadLine(), out int travelChoice) &&
                        travelChoice >= 1 && travelChoice <= ConnectedLocations.Count)
                    {
                        TryEncounter(player, party, engine, "leaving the area");

                        if(!player.IsAlive)
                        {
                            exploring = false;
                        }

                        exploring = false;
                        var next = ConnectedLocations[travelChoice - 1];


                        TryEncounter(player, party, engine, $"traveling to {next.Name}");
                        if (!player.IsAlive)
                        {
                            exploring = false;
                        }
                        next.Enter(player, party, engine);
                    }
                    else
                    { 
                        GameUtils.TypeLine("You decide to stay."); 
                    }
                    break;

                case "5":
                    GameUtils.TypeLine($"{player.Name} and the party take a rest. HP restored!");
                    player.Health = player.MaxHealth;
                    foreach (var companion in party)
                    { 
                        companion.Health = companion.MaxHealth; 
                    }
                    break;

                case "6":
                    GameUtils.TypeLine("You decide to end your journey.");
                    exploring = false;
                    break;

                default:
                    GameUtils.TypeLine("Invalid choice.");
                    break;
            }
        }
    }

    public void TryMeetCompanion(Player player, List<Companion> party)
    {
        if (Companion != null && IsCleared)
        {
            GameUtils.TypeLine($"Wait... hero, you saved me.");
            GameUtils.TypeLine($"My name is {Companion.Name}, I am local {Companion.Role}.");
            GameUtils.TypeLine($"I could be of great help, will you allow me to join your party? \n yes / no");

            var choice = Console.ReadLine().ToLower();
            if (choice == "yes")
            {
                GameUtils.TypeLine($"Thank you, {player.Name}");
                GameUtils.TypeLine($"I promise i will not fail you!");

                party.Add(Companion);
            }
            else
            {               
                GameUtils.TypeLine($"You have rejected {Companion.Name} and you both walked different ways");
            }
        }
        else if (Companion != null && Enemies == null)
        {
            GameUtils.TypeLine($"I've heard of your bravery, {player.Name}. Allow me to aid you on your journey!");
            GameUtils.TypeLine($"My name is {Companion.Name}, I am local {Companion.Role}.");
            GameUtils.TypeLine($"I could be of great help, will you allow me to join your party? \n yes / no");

            var choice = Console.ReadLine().ToLower();
            if (choice == "yes")
            {
                GameUtils.TypeLine($"Thank you, {player.Name}");
                GameUtils.TypeLine($"I promise i will not fail you!");

                party.Add(Companion);
            }
            else
            {
                GameUtils.TypeLine($"You have rejected {Companion.Name} and you both walked different ways");
            }
        }
    }
}
