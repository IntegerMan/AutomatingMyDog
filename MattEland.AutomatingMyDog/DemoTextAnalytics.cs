using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;

namespace MattEland.AutomatingMyDog;

public class DemoTextAnalytics
{
    private readonly TextAnalyticsClient _textClient;

    public DemoTextAnalytics(string key, string endpoint)
    {
        _textClient = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(key));
    }
    
    public void AnalyzeText(string text)
    {
        Console.WriteLine("Using Text Analytics to analyze text...");

        // Detect Language
        Response<DetectedLanguage> response = _textClient.DetectLanguage(text);

        DetectedLanguage lang = response.Value;
        
        Console.WriteLine($"{lang.Name} (ISO Code: {lang.Iso6391Name}): {lang.ConfidenceScore:P}");

        // Detect Key Phrases
        Response<KeyPhraseCollection> keyPhrasesResponse = _textClient.ExtractKeyPhrases(text);
        KeyPhraseCollection keyPhrases = keyPhrasesResponse.Value;

        Console.WriteLine();
        Console.WriteLine("Key Phrases:");
        foreach (string phrase in keyPhrases)
        {
            Console.WriteLine($"\t{phrase}");
        }

        // Detect Entities
        Response<CategorizedEntityCollection> recognizeResponse = _textClient.RecognizeEntities(text);

        Console.WriteLine();
        Console.WriteLine("Entities:");

        CategorizedEntityCollection entities = recognizeResponse.Value;
        foreach (CategorizedEntity entity in entities)
        {
            Console.WriteLine($"\t{entity.Text} (Category: {entity.Category}) with {entity.ConfidenceScore:P} confidence");
        }

        // Detect Linked Entities
        Response<LinkedEntityCollection> linkedResponse = _textClient.RecognizeLinkedEntities(text);

        Console.WriteLine();
        Console.WriteLine("Linked Entities:");

        // Loop over the linked entities recognized
        LinkedEntityCollection linkedEntities = linkedResponse.Value;
        foreach (LinkedEntity entity in linkedEntities)
        {
            Console.WriteLine($"\t{entity.Name} (Url: {entity.Url})");

            // Each linked entity may have multiple matches
            foreach (LinkedEntityMatch match in entity.Matches)
            {
                Console.WriteLine($"\t\tMatched on '{match.Text}' with {match.ConfidenceScore:P} confidence");
            }
        }

        // Detect PII
        Response<PiiEntityCollection> piiResponse = _textClient.RecognizePiiEntities(text);
        PiiEntityCollection piiEntities = piiResponse.Value;

        Console.WriteLine();
        Console.WriteLine("Redacted Text: " + piiEntities.RedactedText);
        Console.WriteLine("PII Entities:");
        
        foreach (PiiEntity entity in piiEntities)
        {
            string category = entity.Category.ToString();
            if (!string.IsNullOrWhiteSpace(entity.SubCategory))
            {
                category += "/" + entity.SubCategory;
            }

            Console.WriteLine($"\t{entity.Text} (Category: {category}) with {entity.ConfidenceScore:P} confidence");
        }

        // Detect Sentiment
        Response<DocumentSentiment> sentimentResponse = _textClient.AnalyzeSentiment(text);
        DocumentSentiment sentiment = sentimentResponse.Value;
        
        Console.WriteLine();
        Console.WriteLine($"Overall Sentiment: {sentiment.Sentiment}");

        // Log confidence scores for the overall sentiment
        Console.WriteLine($"\tPositive: {sentiment.ConfidenceScores.Positive:P}");
        Console.WriteLine($"\tNeutral: {sentiment.ConfidenceScores.Neutral:P}");
        Console.WriteLine($"\tNegative: {sentiment.ConfidenceScores.Negative:P}");

        Console.WriteLine();

        // Log scores for each sentence
        foreach (SentenceSentiment sentence in sentiment.Sentences)
        {
            Console.WriteLine($"'{sentence.Text}' had a sentiment of {sentence.Sentiment}");

            // Log confidence scores for the sentence
            Console.WriteLine($"\tPositive: {sentence.ConfidenceScores.Positive:P}");
            Console.WriteLine($"\tNeutral: {sentence.ConfidenceScores.Neutral:P}");
            Console.WriteLine($"\tNegative: {sentence.ConfidenceScores.Negative:P}");
        }

        InputHelper.PressAnyKey();
    }



}