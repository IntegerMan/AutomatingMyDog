namespace MattEland.AutomatingMyDog.Core; 

public class ChatResult {
    public required string TopIntent { get; init; }
    public required float TopIntentConfidence { get; init; }
    public required List<KeyValuePair<string, float>> ConsideredIntents { get; init; }
}