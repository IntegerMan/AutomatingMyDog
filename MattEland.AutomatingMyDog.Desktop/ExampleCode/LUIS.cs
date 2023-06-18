using System;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;

public async Task<string> MatchUtteranceToIntentAsync(string utterance) {

    // LUIS Settings - again, store in a config file
    string subscriptionKey = "123abc45def67g89h0i12345jk6lmno7";
    string slotId = "Production"; // or "Staging"
    Guid appId = new Guid("3d8c9e1b-7c5f-4e1a-9f3d-6f5a7b2a6d5e"); // Found in Azure

    // Authenticate with Azure
    ApiKeyServiceClientCredentials creds = new(subscriptionKey);
    LUISRuntimeClient client = new(creds);



    // Call out to LUIS to get a prediction
    PredictionRequest request = new(message);
    IPredictionOperations predictions = client.Prediction;
    PredictionResponse predictResult =
        await predictions.GetSlotPredictionAsync(appId, slotId, request, showAllIntents: true);



    // Get the top intent
    Prediction prediction = predictResult.Prediction;
    string topIntent = prediction.TopIntent;



    // List all considered intents and their confidence %
    foreach (KeyValuePair<string, Intent> intent in prediction.Intents) {
        Console.WriteLine($"{intent.Key}: {intent.Value.Score}");
    }
}




public string GetMessageFromMatchedIntent(string intent) {
    switch (intent.ToUpperInvariant()) {
        case "GOOD BOY":
            return "Jester is a good doggo!";

        case "WALK":
            return "Why yes, Jester DOES want to go on a walk!";

        default:
            return "Jester does not understand.";
    }
}
