using System.Net.NetworkInformation;
using System.Threading;

namespace CaveDiver.Dialogue;

public class GameUtils
{
    public static void TypeLine(string text, int delay = 25)
    {
        foreach (var c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }
    public static void TypeLine()
    {
        Console.WriteLine();
    }

    public static void Type(string text, int delay = 25)
    {
        foreach (var c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
    }
}
