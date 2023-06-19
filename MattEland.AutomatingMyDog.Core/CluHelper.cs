using Azure.Core;
using Azure.AI.Language.Conversations;
using Azure;
using System.Text.Json;

namespace MattEland.AutomatingMyDog.Core;

public class CluHelper
{
    private readonly ConversationAnalysisClient _client;

    public CluHelper(string apiKey, Uri endpoint) {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or whitespace.", nameof(apiKey));
        }

        _client = new ConversationAnalysisClient(endpoint, new AzureKeyCredential(apiKey));
    }

    public IEnumerable<AppMessage> AnalyzeText(string text)
    {
        const MessageSource source = MessageSource.CLU;

        // Build a request
        string projectName = "doggo-clu";
        string deploymentName = "KCDC_CLU";
        var data = new {
            analysisInput = new {
                conversationItem = new {
                    text = text,
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
        Response response = _client.AnalyzeConversation(content);

        /* Sample response JSON         
        {
          "kind": "ConversationResult",
          "result": {
            "query": "Are you a good boy?",
            "prediction": {
              "topIntent": "PRAISE",
              "projectKind": "Conversation",
              "intents": [
                {
                  "category": "PRAISE",
                  "confidenceScore": 0.9357563
                },
                {
                  "category": "HELLO",
                  "confidenceScore": 0.88062775
                },
                ...
              ],
              "entities": []
            }
          }
        } */

        // Get top intent from the result
        using JsonDocument json = JsonDocument.Parse(response.ContentStream!);
        JsonElement root = json.RootElement;
        JsonElement prediction = root.GetProperty("result").GetProperty("prediction");

        // Get the top intent
        string topIntent = prediction.GetProperty("topIntent")!.GetString()!;

        // List all possible intents (inlcuding the top intent)
        List<string> intents = new();
        foreach (JsonElement intent in prediction.GetProperty("intents").EnumerateArray()) {
            string intentName = intent.GetProperty("category")!.GetString()!;
            float confidence = intent.GetProperty("confidenceScore").GetSingle();

            intents.Add($"{intentName} ({confidence:P1})");
        }
        yield return new AppMessage(topIntent, source) {
            Items = intents.Take(3)
        };
    }

}
