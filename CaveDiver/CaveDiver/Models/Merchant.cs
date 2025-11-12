using CaveDiver.Dialogue;
using CaveDiver.Engine;

namespace CaveDiver.Models;

public class Merchant
{
    public string Name { get; set; }
    public string Location { get; set; }
    public List <Item> Stock { get; set; }

    public Merchant(string name)
    {
        Name = name;
    }

    public void Trade(Player player, List<Companion> company)
    {
        while(true)
        {
            GameUtils.TypeLine($"Welcome, {player.Name}! I am {Name}. What would you like to do?");
            GameUtils.TypeLine("1. Buy items");
            GameUtils.TypeLine("2. Sell items");
            GameUtils.TypeLine("3. Leave");

            string? input = Console.ReadLine();

            switch(int.Parse(input))
            {
                case 1:
                    BuyItems(player, company);
                    break;
                case 2:
                    SellItems(player, company, this);
                    break;
                case 3:
                    GameUtils.TypeLine("Safe travels, adventurer");
                    return;
                default:
                    GameUtils.TypeLine("Invalid choice");
                    break;
            }
        }
    }

    private void BuyItems(Player player, List<Companion> company)
    {
        GameUtils.TypeLine($"Your balance: {player.Gold} Gold");
        GameUtils.TypeLine("Here’s what I have for sale:");
        for (int i = 0; i < Stock.Count; i++)
        {
            var item = Stock[i];
            GameUtils.TypeLine($"{i + 1}. {item.Name} ({item.StrengthBonus:+#;-#;0} STR, {item.DefenseBonus:+#;-#;0} DEF, {item.IntelligenceBonus:+#;-#;0} INT) — {item.Price} gold");
        }
        GameUtils.TypeLine($"{Stock.Count + 1}. Cancel");

        GameUtils.TypeLine("Choose a number: ");

        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > Stock.Count + 1)
        {
            GameUtils.TypeLine("Invalid choice.");
            return;
        }

        if (choice == Stock.Count + 1)
            return;

        var selectedItem = Stock[choice - 1];
        if (player.Gold < selectedItem.Price)
        {
            GameUtils.TypeLine("You dont have enough gold");
            return;
        }

        GameUtils.TypeLine($"Who should get {selectedItem.Name}?");
        var party = new List<Character>() { player };
        party.AddRange(company);

        for (int i = 0; i < party.Count; i++)
        {
            GameUtils.TypeLine($"{i + 1}.{party[i].Name}");
            GameUtils.TypeLine($" - {party[i].Strength}");
            GameUtils.TypeLine($" - {party[i].Defense}");
            GameUtils.TypeLine($" - {party[i].Intelligence}");
            GameUtils.TypeLine("Inventory:");
            for (int j = 0; j < party[i].Inventory.Count; j++)
            {
                GameUtils.TypeLine($"   {party[i].Inventory[j].Name}");
                GameUtils.TypeLine($"    - STR: {party[i].Inventory[j].StrengthBonus}");
                GameUtils.TypeLine($"    - DEF: {party[i].Inventory[j].DefenseBonus}");
                GameUtils.TypeLine($"    - INT: {party[i].Inventory[j].IntelligenceBonus}");
                GameUtils.TypeLine($"    - Price: {(party[i].Inventory[j].Price / 3) * 2}");
            }
        }
        GameUtils.TypeLine($"{party.Count + 1}. Cancel");

        int characterChoice = GameEngine.AskForNumber($"So who will recieve {selectedItem.Name}?", 1, party.Count + 1);
        var chosenCharacter = party[characterChoice - 1];

        if (!chosenCharacter.AddItem(selectedItem))
        {
            GameUtils.TypeLine($"{chosenCharacter.Name} cannot carry more items.");
            return;
        }

        player.Gold -= selectedItem.Price;

        GameUtils.TypeLine($"You bought {selectedItem.Name} for {selectedItem.Price} gold and gave it to {chosenCharacter.Name}.");
    }

    private void SellItems(Player player, List<Companion> company, Merchant merchant)
    {
        GameUtils.TypeLine("What would you like to sell?");
        var party = new List<Character>() { player };
        party.AddRange(company);

        for (int i = 0; i < party.Count; i++)
        {
            GameUtils.TypeLine($"{i + 1}.{party[i].Name}");
            if(party[i].Inventory.Count == 0)
            {
                GameUtils.TypeLine("   (Empty inventory)");
                continue;
            }
            for (int j = 0; j < party[i].Inventory.Count; j++)
            {
                GameUtils.TypeLine($"   {j + 1}. {party[i].Inventory[j].Name}");
                GameUtils.TypeLine($"    - STR: {party[i].Inventory[j].StrengthBonus}");
                GameUtils.TypeLine($"    - DEF: {party[i].Inventory[j].DefenseBonus}");
                GameUtils.TypeLine($"    - INT: {party[i].Inventory[j].IntelligenceBonus}");
                GameUtils.TypeLine($"    - Sell Price: {(party[i].Inventory[j].Price / 3) * 2}");
            }
        }
        GameUtils.TypeLine($"{party.Count + 1}. Cancel");


        int characterChoice = GameEngine.AskForNumber("Whos inventory will we choose from?", 1, party.Count + 1);
        if (characterChoice == party.Count + 1)
        {
            GameUtils.TypeLine("Sale canceled.");
            return;
        }
        var chosenCharacter = party[characterChoice - 1];

        int itemChoice = GameEngine.AskForNumber($"Which item that {chosenCharacter.Name} holds should be sold?", 1, chosenCharacter.Inventory.Count);
        if (itemChoice == chosenCharacter.Inventory.Count + 1)
        {
            GameUtils.TypeLine("Sale canceled.");
            return;
        }
        var chosenItem = chosenCharacter.Inventory[itemChoice - 1];

        player.Gold += chosenItem.Price / 2;

        merchant.Stock.Add(chosenItem);

        chosenCharacter.RemoveItem(chosenItem);
    }
}
