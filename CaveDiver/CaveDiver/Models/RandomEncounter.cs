using CaveDiver.Dialogue;
using CaveDiver.Engine;
using CaveDiver.Models.Types;
using CaveDiver.Models;

namespace CaveDiver.Models;

public class RandomEncounter
{
    public static void Trigger(Player player, List<Companion> party, Location location)
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

        else if (roll <= 7 && party.Count < 3 && location.Companion == null)
        {
            MeetCompanion(party, location);
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

    private static void MeetCompanion(List<Companion> party, Location location)
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
        var description = SetDescription(name, role);

        var newCompanion = new Companion(name, role, description);

        GameUtils.TypeLine($"\nYou notice a traveler nearby...");
        GameUtils.TypeLine(newCompanion.Description);

        location.Companion = newCompanion;
    }

    private static string SetDescription(string name, string role)
    {
        bool isFemale = name is "Lina" or "Elara" or "Mira" or "Runa";
        string pronoun = isFemale ? "She" : "He";
        string possessive = isFemale ? "her" : "his";

        return role switch
        {
            CompanionRoles.Mage => isFemale
                ? $"{name} stands beneath the trees, arcane energy swirling softly around her fingertips. Her eyes shimmer with ancient knowledge, and a faint aura of magic hums in the air around her."
                : $"{name} studies a worn spellbook, arcane symbols flickering faintly around him. A quiet intensity burns in his eyes — the mark of a seasoned mage.",

            CompanionRoles.Warrior => isFemale
                ? $"{name} sharpens her blade with calm precision. Scars mark her armor, and her steady gaze reveals a fighter forged in countless battles."
                : $"{name} rests a massive weapon against his shoulder. Broad and battle-hardened, he carries himself with the confidence of a veteran warrior.",

            CompanionRoles.Healer => isFemale
                ? $"{name} kneels beside a wounded traveler, her hands glowing with gentle restorative light. A calm warmth radiates from her presence."
                : $"{name} carries a satchel filled with herbs and vials. His touch is steady, and a soothing light gathers as he prepares a healing spell.",

            _ => $"{name} looks like someone who has seen many roads and carries a story worth hearing."
        };
    }
}
