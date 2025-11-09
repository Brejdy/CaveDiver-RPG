using CaveDiver.Engine;
using CaveDiver.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var player = new Player("Grodian", 30, 15, 10, 5);

var companions = new List<Companion>
{
    new Companion("Lina", CompanionRoles.Healer),
    new Companion("Borin", CompanionRoles.Warrior),
    new Companion("Elara", CompanionRoles.Mage)
};

var enemies = new List<Enemy>
{ 
    new Enemy("Goblin 1", EnemyType.Goblin),
    new Enemy("Goblin Shaman 3", EnemyType.GoblinShaman),
};

var game = new GameEngine();
game.StartBattle(player, companions, enemies);