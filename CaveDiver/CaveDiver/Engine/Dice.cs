namespace CaveDiver.Engine;

public static class Dice
{
    private static readonly Random _random = new Random();

    public static int Roll (int sides)
    {
        if (sides < 2)
        {
            throw new ArgumentException("Dice must have at least 2 sides");
        }

        return _random.Next(1, sides + 1);
    }

    public static int D4() => Roll(4);
    public static int D6() => Roll(6);
    public static int D8() => Roll(8);
    public static int D10() => Roll(10);
    public static int D12() => Roll(12);
    public static int D20() => Roll(20);

    public static int RollMultiple(int count, int sides)
    {
        int total = 0;
        for (int i = 0; i < count; i++)
            total += Roll(sides);
        return total;
    }
}
