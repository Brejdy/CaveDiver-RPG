using CaveDiver.Engine;
using CaveDiver.Dialogue;
using CaveDiver.Models;

//TODOS:
//          - Add ollama for conversation

Console.OutputEncoding = System.Text.Encoding.UTF8;

GameUtils.TypeLine("Choose your name HERO:");
var name = Console.ReadLine();

var player = new Player(name, 100, 100, 100, 100); //20, 8, 6, 6
player.Gold = 5;

GameUtils.TypeLine($"Welcome {player.Name} to the world that needs saving.");
GameUtils.TypeLine($"Your stats are:");
GameUtils.TypeLine($"   - Max HP: {player.MaxHealth}");
GameUtils.TypeLine($"   - Strength: {player.Strength}");
GameUtils.TypeLine($"   - Defense: {player.Defense}");
GameUtils.TypeLine($"   - Intelligence: {player.Intelligence}");

GameUtils.TypeLine("To save the world explore all caves and kill Goblin kings");
GameUtils.TypeLine("Durning your journey you will meet companions - do not reject them, they will be a great help!");

var party = new List<Companion>();

var world = new World();

var engine = new GameEngine();

world.StartAdventure(player, party, engine);