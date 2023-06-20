using Azure.Core;
using Azure.AI.Language.Conversations;
using Azure;
using System.Text.Json;

public string GetIntentFromUtterance(string utterance) {

    // Read these from a config file in production
    string endpoint = "https://yourlanguageendpoint.cognitiveservices.azure.com/";
    string apiKey = "123abc45def67g89h0i12345jk6lmno7";

    // Authenticate
    AzureKeyCredential creds = new AzureKeyCredential(apiKey);
    ConversationAnalysisClient client = new(endpoint, creds);



    // You need a project and a deployment, but these have more flexibility
    string projectName = "doggo-clu";
    string deploymentName = "KCDC_CLU";



    // Build a JSON Payload. I hope this part gets better :-(
    var data = new {
        analysisInput = new {
            conversationItem = new {
                text = utterance, // "Why is this strange man automating his dog?",
                modality = "text",
                language = "en-US",
                id = "1",
                participantId = "1",
            }
        },
        parameters = new {
            projectName = projectName,
            verbose = true,
            deploymentName = deploymentName,

            // Use Utf16CodeUnit for strings in .NET.
            stringIndexType = "Utf16CodeUnit",
        },
        kind = "Conversation",
    };



    // Call out to CLU
    RequestContent content = RequestContent.Create(data);
    Response response = client.AnalyzeConversation(content);


    /* Sample response JSON         
    {
        "kind": "ConversationResult",
        "result": {
            "query": "Why is this strange man automating his dog?",
            "prediction": {
                "topIntent": "CALL_THE_COPS",
                "projectKind": "Conversation",
                "intents": [
                {
                    "category": "CALL_THE_COPS",
                    "confidenceScore": 0.9357563
                },
                {
                    "category": "MOSTLY_HARMLESS",
                    "confidenceScore": 0.88062775
                },
                ...
                ],
                "entities": []
            }
        }
    } 
    */


    // Get top intent from the result
    using JsonDocument json = JsonDocument.Parse(response.ContentStream!);
    JsonElement root = json.RootElement;
    JsonElement prediction = root.GetProperty("result").GetProperty("prediction");

    string topIntent = prediction.GetProperty("topIntent")!.GetString()!;



    // List all intents (including top intent) with their confidence scores
    foreach (JsonElement intent in prediction.GetProperty("intents").EnumerateArray()) {
        string intentName = intent.GetProperty("category")!.GetString()!;
        float confidence = intent.GetProperty("confidenceScore").GetSingle();

        Console.WriteLine($"{intentName} ({confidence:P1})");
    }



    // Return the top intent
    return topIntent;
}
