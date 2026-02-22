using CaveDiver.Models;
using CaveDiver.Models.Types;
using CaveDiver.Dialogue;
using CaveDiver.Engine;

namespace CaveDiver.Models;

public class World
{
    public Location StartingLocation { get; private set; }

    public World()
    {
        var willowdale = new Location("Willowdale", LocationType.Village, "A quiet riverside village where travelers rest and trade stories.");
        var oakstead = new Location("Oakstead", LocationType.Village, "A small farming community surrounded by golden wheat fields.");

        var silverpineTown = new Location("Silverpine Town", LocationType.Town, "A bustling merchant town at the heart of the valley, where adventurers gather.");

        var whisperingWoods = new Location("Whispering Woods", LocationType.Forest, "Dark trees whisper in the wind; eyes seem to watch you from the shadows.");
        var emeraldGrove = new Location("Emerald Grove", LocationType.Forest, "Sunlight pierces through the canopy, illuminating peaceful glades filled with wildlife.");
        var shadowThicket = new Location("Shadow Thicket", LocationType.Forest, "Dense undergrowth and thick mist make this forest easy to get lost in.");
        var crimsonHollow = new Location("Crimson Hollow", LocationType.Forest, "A cursed forest where red leaves never fall and the air smells of iron.");

        var twilightCave = new Location("Twilight Cave", LocationType.Cave, "Echoing tunnels shimmer with faint purple light — something stirs within.");
        var frostmawCavern = new Location("Frostmaw Cavern", LocationType.Cave, "Icy stalactites hang like fangs; your breath freezes in the air.");
        var moltenDepths = new Location("Molten Depths", LocationType.Cave, "Rivers of lava light the walls — heat and danger radiate from every crevice.");
        var abyssalChasm = new Location("Abyssal Chasm", LocationType.Cave, "A bottomless pit of darkness; whispers of ancient evil rise from below.");

        oakstead.ConnectedLocations.Add(emeraldGrove);
        oakstead.Merchant = new Merchant("Old Gregor")
        {
            Stock = new List<Item>
            {
                new Item("Sharp knife", "Weapon", str: 1, price: 2),
                new Item("Leather vest", "Armor", def: 1, price: 3)
            }
        };

        willowdale.ConnectedLocations.Add(emeraldGrove);
        willowdale.ConnectedLocations.Add(whisperingWoods);
        willowdale.ConnectedLocations.Add(crimsonHollow);
        willowdale.Merchant = new Merchant("Harvey The Traveler")
        {
            Stock = new List<Item>
            {
                new Item("Silver sword", "Weapon", str: 15, def: 5, price: 30),
                new Item("Plate chestguard", "Armor", str: 5, def: 20, price: 50),
                new Item("Cloak of Medhiv", "Armor", def: 4, intl: 15, price: 45),
                new Item("Magic staff", "Weapon", str: 10, intl: 15, price: 70)
            }
        };
        willowdale.Companion = new Companion("Ardia", CompanionRoles.Mage, "A young mage, a girl in her late teens with long red hair and shifty green eyes");

        emeraldGrove.ConnectedLocations.Add(willowdale);
        emeraldGrove.ConnectedLocations.Add(whisperingWoods);
        emeraldGrove.ConnectedLocations.Add(oakstead);
        emeraldGrove.Enemies = new List<Enemy>
        {
            new Enemy("Goblin Scout", EnemyType.Goblin)
            {
                Health = 10,
                Strength = 5,
                Defense = 3
            }
        };
        emeraldGrove.Companion = new Companion("Lyria", CompanionRoles.Warrior, "Muscle bound woman with battle scars telling a warriors story");
        
        whisperingWoods.ConnectedLocations.Add(emeraldGrove);
        whisperingWoods.ConnectedLocations.Add(twilightCave);
        whisperingWoods.ConnectedLocations.Add(willowdale);
        whisperingWoods.ConnectedLocations.Add(silverpineTown);
        whisperingWoods.Enemies = new List<Enemy>
        {
            new Enemy("Goblin Scout", EnemyType.Goblin),
            new Enemy("Goblin Shaman", EnemyType.GoblinShaman)
        };

        silverpineTown.ConnectedLocations.Add(whisperingWoods);
        silverpineTown.ConnectedLocations.Add(frostmawCavern);
        silverpineTown.ConnectedLocations.Add(shadowThicket);
        silverpineTown.ConnectedLocations.Add(crimsonHollow);
        silverpineTown.Merchant = new Merchant("Goldie Flowisca")
        {
            Stock = new List<Item>
            {
                new Item("Sword of Colitha", "Weapon", str: 40, def: 20, intl: 5, price: 200),
                new Item("Full-plate armor", "Armor", str: 10, def: 50, intl: 5, price: 400),
                new Item("Magic wand of Thriora", "Weapon", str: 15, def: 5, intl: 30, price: 400),
                new Item("Healers gown", "Armor", def: 5, intl: 50, price: 500),
            }
        };

        shadowThicket.ConnectedLocations.Add(silverpineTown);
        shadowThicket.ConnectedLocations.Add(moltenDepths);
        shadowThicket.Enemies = new List<Enemy>
        {
            new Enemy("Goblin Scout", EnemyType.Goblin),
            new Enemy("Goblin Shaman", EnemyType.GoblinShaman),
            new Enemy("Goblin Scout", EnemyType.Goblin),
            new Enemy("Hobgoblin", EnemyType.Hobgoblin)
        };

        crimsonHollow.ConnectedLocations.Add(silverpineTown);
        crimsonHollow.ConnectedLocations.Add(willowdale);
        crimsonHollow.ConnectedLocations.Add(abyssalChasm);
        crimsonHollow.Enemies = new List<Enemy>
        {
             new Enemy("Hobgoblin", EnemyType.Hobgoblin),
             new Enemy("Hobgoblin", EnemyType.Hobgoblin)
        };

        twilightCave.ConnectedLocations.Add(whisperingWoods);
        twilightCave.Enemies = new List<Enemy>
        {
             new Enemy("Goblin warrior", EnemyType.Goblin),
             new Enemy("Goblin warrior", EnemyType.Goblin),
             new Enemy("Goblin General", EnemyType.General)
        };

        frostmawCavern.ConnectedLocations.Add(silverpineTown);
        frostmawCavern.Enemies = new List<Enemy>
        {
             new Enemy("Hobgoblin", EnemyType.Hobgoblin),
             new Enemy("Goblin General", EnemyType.General)
        };

        moltenDepths.ConnectedLocations.Add(silverpineTown);
        moltenDepths.Enemies = new List<Enemy>
        {
             new Enemy("Goblin King", EnemyType.King),
             new Enemy("Goblin warrior", EnemyType.Goblin),
             new Enemy("Goblin warrior", EnemyType.Goblin)
        };

        abyssalChasm.ConnectedLocations.Add(silverpineTown);
        abyssalChasm.Enemies = new List<Enemy>
        {
             new Enemy("Hobgoblin", EnemyType.Hobgoblin),
             new Enemy("Goblin King", EnemyType.King)
             
        };

        StartingLocation = oakstead;
    }

    public void StartAdventure(Player player, List<Companion> party, GameEngine engine)
    {
        GameUtils.TypeLine("Your Journey begins...");
        StartingLocation.Enter(player, party, engine);
    }
}
