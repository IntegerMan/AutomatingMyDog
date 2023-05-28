using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;

namespace MattEland.AutomatingMyDog.Core;

public class LanguageUnderstandingHelper
{
    private readonly LUISRuntimeClient _luisClient;
    private readonly Guid _appId;
    private readonly string _slotId;

    public LanguageUnderstandingHelper(string subscriptionKey, string endpoint, Guid appId, string slotId = "Production")
    {
        if (string.IsNullOrWhiteSpace(subscriptionKey))
        {
            throw new ArgumentException($"'{nameof(subscriptionKey)}' cannot be null or whitespace.", nameof(subscriptionKey));
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or whitespace.", nameof(endpoint));
        }

        if (string.IsNullOrWhiteSpace(slotId))
        {
            throw new ArgumentException($"'{nameof(slotId)}' cannot be null or whitespace.", nameof(slotId));
        }

        _luisClient = new LUISRuntimeClient(new ApiKeyServiceClientCredentials(subscriptionKey))
        {
            Endpoint = endpoint
        };

        _appId = appId;
        _slotId = slotId; // Slots are typically Production or Staging
    }

    public IEnumerable<AppMessage> AnalyzeText(string text)
    {
        const MessageSource source = MessageSource.LanguageUnderstanding;

        // Call out to LUIS
        PredictionRequest request = new(text);
        PredictionResponse predictResult = _luisClient.Prediction.GetSlotPredictionAsync(_appId, _slotId, request, showAllIntents: true, verbose: false).Result;

        // Display the top intent first
        yield return new AppMessage($"Top Intent: {predictResult.Prediction.TopIntent} with {predictResult.Prediction.Intents[predictResult.Prediction.TopIntent].Score!.Value:P2} confidence", source);

        // Display other considered intents
        foreach (KeyValuePair<string, Intent> intent in predictResult.Prediction.Intents.OrderByDescending(i => i.Value.Score))
        {
            // Don't display the top intent
            if (intent.Key == predictResult.Prediction.TopIntent)
            {
                continue;
            }

            yield return new AppMessage($"Possible Intent: {intent.Key} with {intent.Value.Score!.Value:P2} confidence", source);
        }

        // Respond to the top intent
        switch (predictResult.Prediction.TopIntent.ToUpperInvariant())
        {
            case "GOOD BOY":
                yield return new AppMessage("Jester is a good doggo!", MessageSource.DogOS);
                break;

            case "WALK":
                yield return new AppMessage("Why yes, Jester DOES want to go on a walk!", MessageSource.DogOS);
                break;

            default:
                yield return new AppMessage("Jester does not understand.", MessageSource.DogOS);
                break;
        }
    }

}
