using CaveDiver.Dialogue;

namespace CaveDiver.Models;

public class Player : Character
{
    public int Gold { get; set; }
    public Player(string name, int maxHealth, int strength, int defense, int intelligence) : base(name, maxHealth, maxHealth, strength, defense, intelligence)
    { }

    public override void GainExperience(int amount)
    {
        base.GainExperience(amount);
        int nextLevel = Level * 100;
        while (Experience > nextLevel)
        {
            Level++;
            Experience -= nextLevel;

            MaxHealth += 5;
            Health = MaxHealth;

            LevelUpChoice();

            GameUtils.TypeLine($"{Name} has leveled up to Level {Level}");
            GameUtils.TypeLine($"New stats are:\n Max Health: {MaxHealth} \n Strength: {Strength} \n Defense: {Defense} \n Inteligence: {Intelligence}");
        }
    }

    private void LevelUpChoice()
    {
        int points = 10;
        GameUtils.TypeLine("You have gained 10 stat points to distribute!");
        GameUtils.TypeLine("Choose wisely...");

        int pointsGiven = 0;

        while (points > 0)
        {
            GameUtils.TypeLine($"Points remaining: {points}");
            GameUtils.TypeLine("Where would you like to invest?");
            GameUtils.TypeLine("Choose:\n 1 for Strength \n 2 for Defense \n 3 for Intelligence");

            string? choice = Console.ReadLine();


            switch (choice)
            {
                case "1":
                    GameUtils.TypeLine($"You have chosen strength.");
                    GameUtils.TypeLine($"How many ponit do you want to invest into strength?");
                    pointsGiven = int.Parse(Console.ReadLine());
                    if (points - pointsGiven < 0)
                    {
                        GameUtils.TypeLine("Bevare hero, you are tapping into forces you dont want to play with!");
                        GameUtils.TypeLine("Spend only point you have available!");
                        break;
                    }
                    Strength += pointsGiven;
                    points -= pointsGiven;
                    break;
                
                case "2":
                    GameUtils.TypeLine($"You have chosen defence.");
                    GameUtils.TypeLine($"How many ponit do you want to invest into defence?");
                    pointsGiven = int.Parse(Console.ReadLine());
                    if (points - pointsGiven < 0)
                    {
                        GameUtils.TypeLine("Bevare hero, you are tapping into forces you dont want to play with!");
                        GameUtils.TypeLine("Spend only point you have available!");
                        break;
                    }
                    Defense += pointsGiven;
                    points -= pointsGiven;
                    break;
                
                case "3":
                    GameUtils.TypeLine($"You have chosen intelligence.");
                    GameUtils.TypeLine($"How many ponit do you want to invest into intelligence?");
                    pointsGiven = int.Parse(Console.ReadLine());
                    if (points - pointsGiven < 0)
                    {
                        GameUtils.TypeLine("Bevare hero, you are tapping into forces you dont want to play with!");
                        GameUtils.TypeLine("Spend only point you have available!");
                        break;
                    }
                    Intelligence += pointsGiven;
                    points -= pointsGiven;
                    break;
                default:
                    GameUtils.TypeLine("Invalid choice. Try again.");
                    break;
            }
            Thread.Sleep(500);
        }
        GameUtils.TypeLine("Level Up complete!");
        Thread.Sleep(1000);
    }
}
