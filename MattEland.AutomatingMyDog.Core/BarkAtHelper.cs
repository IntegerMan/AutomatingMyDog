namespace MattEland.AutomatingMyDog.Core;

public static class BarkAtHelper
{
    public static bool IsSomethingToBarkAt(this string? thing)
    {
        // Protect against null and mixed casing
        thing = thing?.ToLowerInvariant() ?? "";

        return thing.Contains("squirrel") ||
               thing.Contains("rabbit") ||
               thing.Contains("rodent") ||
               thing.Contains("toy") ||
               thing.Contains("stuffed toy") ||
               thing.Contains("plush") ||
               thing.Contains("bear") ||
               thing.Contains("raccoon") ||
               thing.Contains("possum") ||
               thing.Contains("cat") ||
               thing.Contains("animal") ||
               thing.Contains("bird") ||
               thing.Contains("dog");
    }
}
