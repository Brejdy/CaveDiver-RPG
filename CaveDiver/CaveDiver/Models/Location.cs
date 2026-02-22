using CaveDiver.Dialogue;
using CaveDiver.Models.Types;
using CaveDiver.Engine;
using CaveDiver.Interfaces;

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
                    ambushEnemies.Add(new Enemy("Bandit", EnemyType.Goblin));
            }
            else if (Type == LocationType.Cave)
            {
                ambushEnemies.Add(new Enemy("Hobgoblin", EnemyType.Hobgoblin));
                if (Dice.Roll(10) > 6)
                    ambushEnemies.Add(new Enemy("Goblin General", EnemyType.General));
                if (Dice.Roll(10) > 9)
                    ambushEnemies.Add(new Enemy("Goblin King", EnemyType.King));
            }

            var survived = engine.StartBattle(player, party, ambushEnemies);
            if (!survived)
            {
                return;
            }
        }
        else
        {
            RandomEncounter.Trigger(player, party, this);
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
            GameUtils.TypeLine("1. Look around (or type: look)");
            if (Merchant != null)
            { 
                GameUtils.TypeLine("2. Trade with merchant (or type: trade)"); 
            }
            if (Enemies.Any())
            {
                GameUtils.TypeLine("3. Engage in battle (or type: battle/attack)");
            }
            if (ConnectedLocations.Any())
            {
                GameUtils.TypeLine("4. Travel to another location (or type: travel/go to ... )");
            }
            GameUtils.TypeLine("5. Rest (or type: rest)");
            GameUtils.TypeLine("6. Leave game (or type: exit)");
            
            if (party.Count > 0)
            {
                GameUtils.TypeLine("7. Talk to companion (or type talk");
            }

            string? input = Console.ReadLine();
            var action = CommandParser.ParseExplorationAction(input, Merchant != null, Enemies.Any(), ConnectedLocations.Any());

            switch (action)
            {
                case CommandParser.ExplorationAction.LookAround:
                    
                    GameUtils.TypeLine($"You look around. {Description}"); 

                    if (Companion != null)
                    {
                        TryMeetCompanion(player, party);
                    }
                    
                    break;

                case CommandParser.ExplorationAction.Trade:
                    Merchant?.Trade(player, party);
                    break;

                case CommandParser.ExplorationAction.Battle:
                    if (Enemies.Any())
                    {
                        GameUtils.TypeLine("There are enemies nearby, they have spotted you!");
                        TryEncounter(player, party, engine, "entering the area");
                        var survived = engine.StartBattle(player, party, Enemies);

                        if (!survived)
                        {
                            return;
                        }

                        Enemies.Clear();
                        IsCleared = true;

                        TryMeetCompanion(player, party);
                    }
                    else
                    {
                        GameUtils.TypeLine("There are no enemies left here.");
                        TryMeetCompanion(player, party);
                    }
                    break;

                case CommandParser.ExplorationAction.Travel:
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
                    GameUtils.TypeLine($"{ConnectedLocations.Count + 1}. Stay here (or type: stay)");

                    var travelInput = Console.ReadLine();
                    if (CommandParser.TryResolveTravelDestination(travelInput, ConnectedLocations, out int travelIndex, out bool stayHere))
                    {
                        if (stayHere)
                        {
                            GameUtils.TypeLine("You decide to stay.");
                            break;
                        }

                        TryEncounter(player, party, engine, "leaving the area");

                        if(!player.IsAlive)
                        {
                            exploring = false;
                        }

                        exploring = false;
                        var next = ConnectedLocations[travelIndex];


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

                case CommandParser.ExplorationAction.Rest:
                    GameUtils.TypeLine($"{player.Name} and the party set up camp.");
                    GameUtils.TypeLine($"After building a fire, eating hearty meal and sharing stories you all feel rested.");
                    GameUtils.TypeLine($"HP has been restored.");
                    player.Health = player.MaxHealth;
                    foreach (var companion in party)
                    { 
                        companion.Health = companion.MaxHealth; 
                    }
                    break;

                case CommandParser.ExplorationAction.Exit:
                    GameUtils.TypeLine("You decide to end your journey.");
                    exploring = false;
                    break;

                case CommandParser.ExplorationAction.Talk:
                    StartConversation(player, party);
                    break;

                default:
                    GameUtils.TypeLine("Invalid action. Try text commands like 'look', 'trade', 'battle', 'travel', 'rest', 'exit'.");
                    break;
            }
        }
    }

    public void TryMeetCompanion(Player player, List<Companion> party)
    {
        if (Companion == null)
            return;

        if (HasEnemies)
        {
            GameUtils.TypeLine($"\nThrough the chaos of battle, you notice a {Companion.Description}, a {Companion.Role} trapped nearby!");
            GameUtils.TypeLine($"Defeat the enemies to save the {Companion.Role}.");
            return;
        }

        if (IsCleared)
        {
            GameUtils.TypeLine("\nYou notice someone approaching you...");
            GameUtils.TypeLine(Companion.Description);
            GameUtils.TypeLine("\nWait... hero, you saved me.");
        }
        else
        {
            GameUtils.TypeLine("\nA figure approaches you calmly...");
            GameUtils.TypeLine(Companion.Description);
        }

        GameUtils.TypeLine($"\nMy name is {Companion.Name}, I am a {Companion.Role}.");
        GameUtils.TypeLine("Will you allow me to join your party? (yes / no)");

        var choice = Console.ReadLine()?.ToLower();

        if (choice == "yes")
        {
            GameUtils.TypeLine($"Thank you, {player.Name}.");
            GameUtils.TypeLine("I promise I will not fail you!");

            party.Add(Companion);
            Companion = null;
        }
        else
        {
            GameUtils.TypeLine($"You and {Companion.Name} walk different paths.");
            Companion = null;
        }
    }

    private async Task StartConversation(Player player, List<Companion> party)
    {
        if (!party.Any())
        {
            GameUtils.TypeLine("You have no companions to talk to.");
            return;
        }

        GameUtils.TypeLine("Who do you want to talk to?");
        for (int i = 0; i < party.Count; i++)
        {
            GameUtils.TypeLine($"{i + 1}. {party[i].Name}");
        }

        int choice = GameEngine.AskForNumber("Enter number: ", 1, party.Count);
        var companion = party[choice - 1];

        IDialogueProvider provider = new DialogueProvider();

        GameUtils.TypeLine($"\nYou start talking with {companion.Name}.");
        GameUtils.TypeLine("Type 'bye' to end conversation.\n");

        while (true)
        {
            GameUtils.Type("You: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.ToLower() == "bye")
            {
                GameUtils.TypeLine($"{companion.Name}: Until next time.");
                break;
            }

            var context = new DialogueContext
            {
                Player = player,
                Companion = companion,
                PlayerInput = input
            };

            var response = await provider.GetResponseAsync(context);

            companion.Remember(input, response);

            GameUtils.TypeLine($"{companion.Name}: {response}");
            GameUtils.TypeLine();
        }
    }
}
