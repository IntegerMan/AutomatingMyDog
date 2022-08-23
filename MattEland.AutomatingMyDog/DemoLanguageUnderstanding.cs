using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;

namespace MattEland.AutomatingMyDog;

public class DemoLanguageUnderstanding
{
    private readonly LUISRuntimeClient _luisClient;
    private readonly string _slotId;
    private readonly Guid _appId;

    public DemoLanguageUnderstanding(string subscriptionKey, 
        string endpoint, 
        string appId, 
        string slotId = "Production")
    {
        _luisClient = new LUISRuntimeClient(new ApiKeyServiceClientCredentials(subscriptionKey));
        _luisClient.Endpoint = endpoint;

        _appId = new Guid(appId);
        _slotId = slotId; // Slots are typically production or staging
    }

    public string DetectIntent(string message, 
        string unknownIntentName = "Unknown", 
        double recognizedIntentThreshold = 0.6)
    {
        // Call out to LUIS
        PredictionRequest request = new(message);

        PredictionResponse predictResult =
            _luisClient.Prediction.GetSlotPredictionAsync(_appId, _slotId, request).Result;

        Prediction prediction = predictResult.Prediction;

        // Analyzed the matched intent(s)
        string topIntent = DetermineTopIntent(prediction, unknownIntentName, recognizedIntentThreshold);

        Console.WriteLine();
        Console.WriteLine("Top intent was: " + topIntent);

        return topIntent;
    }

    private static string DetermineTopIntent(Prediction prediction,
        string unknownIntentName = "Unknown",
        double recognizedIntentThreshold = 0.6)
    {
        Console.WriteLine();
        Console.WriteLine("Intents: ");
        foreach (KeyValuePair<string, Intent> intent in prediction.Intents)
        {
            Console.WriteLine(intent.Key + ": " + intent.Value.Score);
        }

        string topIntent = prediction.TopIntent;

        // Ensure we only react to intents over a specific threshold
        if (prediction.Intents.Max(i => i.Value.Score) < recognizedIntentThreshold)
        {
            topIntent = unknownIntentName;
        }

        return topIntent;
    }
}