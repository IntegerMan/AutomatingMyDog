using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace MattEland.AutomatingMyDog;

public class DemoTextAnalytics
{
    private readonly TextAnalyticsClient _textClient;

    public DemoTextAnalytics(string key, string endpoint)
    {
        _textClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(key));
        _textClient.Endpoint = endpoint;
    }
    
    public void AnalyzeText(string text)
    {
        Console.WriteLine("Using Text Analytics to analyze text...");

        // Detect Language
        LanguageResult? langResult = _textClient.DetectLanguageAsync(text, countryHint: "US").Result;
        string languageCode = "en";
        if (langResult != null && langResult.DetectedLanguages.Any())
        {
            Console.WriteLine();
            Console.WriteLine("Detected Language");
            foreach (DetectedLanguage lang in langResult.DetectedLanguages)
            {
                Console.WriteLine($"{lang.Name}: {lang.Score:P}");
            }

            languageCode = langResult.DetectedLanguages.First().Iso6391Name;
        }

        // Detect Key Phrases
        KeyPhraseResult result = _textClient.KeyPhrasesAsync(text, language: languageCode).Result;
        if (result.KeyPhrases.Any())
        {
            Console.WriteLine();
            Console.WriteLine("Key Phrases:");
            foreach (string phrase in result.KeyPhrases)
            {
                Console.WriteLine($"\t{phrase}");
            }
        }

        // Detect Entities
        EntitiesResult entities = _textClient.EntitiesAsync(text, language: languageCode).Result;
        if (entities.Entities.Any())
        {
            Console.WriteLine();
            Console.WriteLine("Entities:");
            foreach (EntityRecord entity in entities.Entities)
            {
                Console.WriteLine($"\t{entity.Name}");
            }
        }

        // Detect Sentiment
        SentimentResult sentimentResult = _textClient.SentimentAsync(text, language: languageCode).Result;
        if (sentimentResult.Score.HasValue)
        {
            Console.WriteLine();
            Console.WriteLine($"Sentiment: {sentimentResult.Score}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }



}