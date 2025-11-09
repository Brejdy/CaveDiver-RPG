using CaveDiver.Engine;
using CaveDiver.Dialogue;

namespace CaveDiver.Models;

public class Companion : Character
{
    public string Role {  get; set; }

    public Companion(string name, string role) : base (name, 0, 0, 0, 0, 0)
    {
        Name = name;
        Role = role;

        switch (Role)
        {
            case CompanionRoles.Healer:
                Health = 25;
                MaxHealth = 30;
                Strength = 2;
                Defense = 8;
                Intelligence = 10;
                break;

            case CompanionRoles.Mage:
                Health = 25;
                MaxHealth = 30;
                Strength = 6;
                Defense = 8;
                Intelligence = 6;
                break;

            case CompanionRoles.Warrior:
                Health = 35;
                MaxHealth = 40;
                Strength = 12;
                Defense = 8;
                Intelligence = 0;
                break;

            default:
                throw new ArgumentException($"Unknown role {role}");
        }
    }

    public void Support(Character target)
    {
        if (Role is CompanionRoles.Healer or CompanionRoles.Mage)
        {
            int heal = Intelligence + Dice.Roll(8) / 2;
            if (target.Health + heal > MaxHealth)
            {
                target.Health = MaxHealth;
            }
            else
                target.Health += heal;

            GameUtils.TypeLine($"{target.Name} has been healed to {target.Health}/{target.MaxHealth} HP");
        }
    }   

    public void Revive(Character target)
    {
        if (Role == CompanionRoles.Healer && !target.IsAlive)
        {
            int revive = Dice.Roll(20);
            GameUtils.Type($"You rolled {revive} ");
            if (revive >= 15)
            {
                target.Health = target.MaxHealth / 2;
                GameUtils.TypeLine($"{target.Name} revived successfully");
            }
            else
            {
                GameUtils.TypeLine("Revival failed");
            }
        }
    }

    public void TakeTurn(Player player, List<Companion> companions, Enemy enemy)
    {
        if (!IsAlive)
        {
            return;
        }

        if (Role == CompanionRoles.Healer)
        {
            var deadCompanion = companions.FirstOrDefault(c => !c.IsAlive);
            if (player.Health < player.MaxHealth / 2)
            {
                Support(player);
                GameUtils.TypeLine("Hang in there");
            }
            else if (deadCompanion != null)
            {
                GameUtils.TypeLine($"Return to us {deadCompanion.Name}");
                Revive(deadCompanion);
            }
            else
            {
                GameUtils.TypeLine("Take this");
                int dmg = Attack();
                enemy.TakeDamage(dmg);
            }
        }
        else if (Role == CompanionRoles.Mage)
        {
            int spellRoll = Dice.Roll(10);
            if (spellRoll >= 5)
            {
                GameUtils.TypeLine("Spirits of lightning hear out my cries and strike my enemy");
                int dmg = Attack() + Intelligence;
                enemy.TakeDamage(dmg);
            }
            else
            {
                GameUtils.TypeLine($"{Name} is focusing but the spell has failed");
            }
        }
        else if (Role == CompanionRoles.Warrior)
        {
            int rollHeavyAttack = Dice.Roll(20);
            if (rollHeavyAttack > 15)
            {
                GameUtils.TypeLine("Try withstanding Heavy Slash");
                var dmg = Attack() * 1.5;
                enemy.TakeDamage((int)dmg);
            }
            else
            {
                var dmg = Attack();
                enemy.TakeDamage(dmg);
            }
        }
    }

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

            switch (Role)
            {
                case CompanionRoles.Mage:
                    Intelligence += 2;
                    Strength += 4;
                    Defense += 4;
                    break;

                case CompanionRoles.Healer:
                    Intelligence += 4;
                    Strength += 1;
                    Defense += 5;
                    break;

                case CompanionRoles.Warrior:
                    Strength += 5;
                    Defense += 5;
                    break;

                default:
                    throw new Exception("Invalid role");
            }


            GameUtils.TypeLine($"{Name} has leveled up to Level {Level}");
            GameUtils.TypeLine($"New stats are:\n Max Health: {MaxHealth} \n Strength: {Strength} \n Defense: {Defense} \n Inteligence: {Intelligence}");
        }
    }
}
