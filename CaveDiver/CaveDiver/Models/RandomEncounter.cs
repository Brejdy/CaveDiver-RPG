using CaveDiver.Dialogue;
using CaveDiver.Engine;
using CaveDiver.Models.Types;
using System.Xml.Linq;

namespace CaveDiver.Models;

public class RandomEncounter
{
    public static void Trigger(Player player, List<Companion> party)
    {
        int roll = Dice.Roll(10);

        if (roll <= 3)
        {
            CalmEvent();
        }

        else if (roll <= 5)
        {
             FindTreasure(player);
        }

        else if (roll <= 7 && party.Count < 3)
        {
            MeetCompanion(party);
        }
        else
        {
            GameUtils.TypeLine("The path is uneventful, and your journey continues...");
        }
    }

    private static void CalmEvent()
    {
        string[] calmTexts =
        {
            "You hear a calm stream flowing nearby.",
            "Birdsong fills the air, easing your tension.",
            "The light through the trees paints golden shapes on the ground.",
            "A warm breeze carries the scent of pine and moss.",
            "The path is quiet. Only your footsteps echo.",
            "You hear distant animal cries, but nothing approaches.",
            "Leaves rustle above, but it’s just the wind.",
            "The air is still. It feels like something is watching… but nothing happens.",
            "You step over old bones half-buried in the soil.",
            "You take a deep breath. For now, the forest is calm."
        };

        var text = calmTexts[Dice.Roll(calmTexts.Length - 1)];
        GameUtils.TypeLine(text);
    }

    private static void FindTreasure(Player player)
    {
        var foundGold = Dice.Roll(20) + 10;
        player.Gold += foundGold;

        GameUtils.TypeLine($"You find a small chest hidden under leaves! It contains {foundGold} gold!");
    }

    private static void MeetCompanion(List<Companion> party)
    {
        string[] possibleNames = { "Lina", "Borin", "Elara", "Toren", "Mira", "Kael", "Runa", "Gromdir" };
        string[] possibleRoles = { CompanionRoles.Mage, CompanionRoles.Warrior, CompanionRoles.Healer };

        string name;
        int safety = 0;

        do
        {
            name = possibleNames[Dice.Roll(possibleNames.Length) - 1];
            safety++;
        } while (party.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) && safety < 20);

        if (party.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            GameUtils.TypeLine("You encounter a traveler, but your party is already full of unique heroes.");
            return;
        }

        var role = possibleRoles[Dice.Roll(possibleRoles.Length) - 1];
        var newCompanion = new Companion(name, role);

        GameUtils.TypeLine($"You meet {name}, a weary traveler ({role}) who offers to join your quest!");
        party.Add(newCompanion);
    }
}
