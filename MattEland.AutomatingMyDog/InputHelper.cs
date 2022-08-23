namespace MattEland.AutomatingMyDog;

public static class InputHelper
{
    public static void PressAnyKey()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        
        Console.WriteLine();
    }
}