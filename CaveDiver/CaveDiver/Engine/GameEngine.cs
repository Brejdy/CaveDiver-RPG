using CaveDiver.Models;
using CaveDiver.Dialogue;

namespace CaveDiver.Engine;

public class GameEngine
{
    public void StartBattle(Player player, List<Companion> companions, List<Enemy> enemies)
    {
        var enemyTypes = string.Join(", ", enemies.Select(e => e.Type).Distinct());
        if (enemies.Count() > 1)
        {
            GameUtils.TypeLine($"Battle begins! {player.Name} and his {companions.Count} companion(s) face a group of {enemies.Count} monsters: {enemyTypes}!");
        }
        else
        {
            GameUtils.TypeLine($"Battle begins! {player.Name} and his {companions.Count} companion(s) face a {enemyTypes}!");
        }
        Console.WriteLine("-------------------------------------------------\n");

        while (player.IsAlive && enemies.Any(e => e.IsAlive))
        {
            GameUtils.Type("Your action (attack enemy no. / run ):");
            string? input = Console.ReadLine()?.ToLower();

            if (input == "run")
            {
                var roll = Dice.Roll(20);
                GameUtils.TypeLine($"Rolling for your escape comes up to: {roll}");

                if (roll > 10)
                {
                    GameUtils.TypeLine("You have successfully ran away");
                    break;
                }
                else
                {
                    GameUtils.TypeLine("The enemy does not let you run away");
                }
            }


            if (input.StartsWith("attack"))
            {
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int targetIndex = (parts.Length > 1 && int.TryParse(parts[1], out int index)) ? index - 1 : 0;
                targetIndex = Math.Clamp(targetIndex, 0, enemies.Count - 1);

                var target = enemies[targetIndex];
                if (!target.IsAlive)
                {
                    GameUtils.TypeLine($"{target.Name} {targetIndex} has already been slain");
                }
                else
                {
                    int dmg = player.Attack();
                    target.TakeDamage(dmg);
                }
            }
            GameUtils.TypeLine();
            Thread.Sleep(1000);

            foreach (var companion in companions.Where(c => c.IsAlive))
            {
                var livingEnemies = enemies.Where(e => e.IsAlive).ToList();
                if (!livingEnemies.Any())
                {
                    break;
                }

                var enemy = livingEnemies.Count == 1
                    ? livingEnemies[0]
                    : livingEnemies[Dice.Roll(livingEnemies.Count) - 1];
                companion.TakeTurn(player, companions, enemy);
                GameUtils.TypeLine();
                Thread.Sleep(1000);
            }

            foreach (var enemy in enemies.Where(e => e.IsAlive))
            {
                var allTargets = new List<Character>() { player };
                allTargets.AddRange(companions.Where((c) => c.IsAlive));

                var target = allTargets.Count == 1
                    ? allTargets[0]
                    : allTargets[Dice.Roll(allTargets.Count) - 1];

                int dmg = enemy.Attack();
                GameUtils.TypeLine($"{enemy.Name} attacks {target.Name}!");
                target.TakeDamage(dmg);
                GameUtils.TypeLine();
                Thread.Sleep(1000);
            }

            GameUtils.TypeLine($"{player.Name} - {player.Health}/{player.MaxHealth} HP");
            foreach (var companion in companions)
            {
                if (!companion.IsAlive)
                {
                    GameUtils.Type($"{companion.Name} - DEAD // ");
                }
                else
                { 
                    GameUtils.Type($"{companion.Name} - {companion.Health}/{companion.MaxHealth} HP // "); 
                }
            }
            GameUtils.TypeLine();
            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive)
                {
                    GameUtils.Type($"{enemy.Name} - DEAD // ");
                }
                else
                {
                    GameUtils.Type($"{enemy.Name} - {enemy.Health}/{enemy.MaxHealth} HP // ");
                }
            }
            GameUtils.TypeLine();
        }

        GameUtils.TypeLine();

        if (!player.IsAlive)
        {
            GameUtils.TypeLine($"{player.Name} has died in battle \n");
            GameUtils.TypeLine("GAME OVER");
        }
        else if (enemies.All(e => !e.IsAlive))
        {
            GameUtils.TypeLine($"All enemies have been slain");

            int totalXP = enemies.Select(e => e.ExperienceReward).Sum();
            int partySize = companions.Count() + 1;
            player.GainExperience(totalXP / partySize);
            player.Gold += enemies.Count() * Dice.D4();

            foreach (var companion in companions)
            {
                if (!companion.IsAlive)
                {
                    GameUtils.Type($"{companion.Name} return to us!");
                    companion.Health = companion.MaxHealth / 5;
                }

                companion.GainExperience(totalXP / partySize);
            }

            var party = new List<Character> { player };
            party.AddRange(companions);

            foreach (var enemy in enemies)
            {
                if (enemy.LootItem != null)
                {
                    GameUtils.TypeLine($"You found {enemy.LootItem.Name} ({enemy.LootItem.ToString()}) on {enemy.Name}!");
                    GameUtils.TypeLine("Who should take this item?");

                    for (int i = 0; i < party.Count; i++)
                    {
                        GameUtils.TypeLine($"{i + 1}. {party[i].Name} \n SRT: {party[i].Strength} \n DEF: {party[i].Defense} \n INT: {party[i].Intelligence}");
                    }

                    int choice = -1;

                    while (choice < 1 || choice > party.Count)
                    {
                        GameUtils.TypeLine("Enter the number of character: ");
                        string? input = Console.ReadLine();
                        if (!int.TryParse(input, out choice) || choice < 1 || choice > party.Count)
                        {
                            GameUtils.TypeLine("Invalid choice, try again!");
                        }
                    }

                    var reciever = party[choice - 1];
                    reciever.AddItem(enemy.LootItem);
                    GameUtils.TypeLine($"{reciever.Name} recieved {enemy.LootItem.Name}");
                }
            }
        }
        else
        {
            GameUtils.TypeLine("Battle is over");
        }
    }
}
