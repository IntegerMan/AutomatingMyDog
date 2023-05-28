using Azure.AI.TextAnalytics;
using Azure;

namespace MattEland.AutomatingMyDog.Core;

public class TextAnalyticsHelper
{
    private readonly TextAnalyticsClient _textClient;

    public TextAnalyticsHelper(string subscriptionKey, string endpoint)
    {
        if (string.IsNullOrWhiteSpace(subscriptionKey))
        {
            throw new ArgumentException($"'{nameof(subscriptionKey)}' cannot be null or whitespace.", nameof(subscriptionKey));
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or whitespace.", nameof(endpoint));
        }

        _textClient = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(subscriptionKey));
    }

    public IEnumerable<string> AnalyzeText(string text)
    {
        // Detect Language
        Response<DetectedLanguage> langResponse = _textClient.DetectLanguage(text);
        DetectedLanguage language = langResponse.Value;
        yield return $"Detected Language {language.Name} ({langResponse.Value.Iso6391Name}) with confidence of {langResponse.Value.ConfidenceScore:P1}";

        // Detect Key Phrases
        Response<KeyPhraseCollection> keyPhrasesResponse = _textClient.ExtractKeyPhrases(text);
        KeyPhraseCollection keyPhrases = keyPhrasesResponse.Value;
        yield return $"Key Phrases: {string.Join(", ", keyPhrases)}";

        // Detect Entities
        Response<CategorizedEntityCollection> recognizeResponse = _textClient.RecognizeEntities(text);
        CategorizedEntityCollection entities = recognizeResponse.Value;
        if (entities.Any())
        {
            foreach (CategorizedEntity entity in entities)
            {
                yield return $"Detected entity '{entity.Text}' (Category: {entity.Category}) with {entity.ConfidenceScore:P1} confidence";
            }
        }

        // Detect Linked Entities
        Response<LinkedEntityCollection> linkedResponse = _textClient.RecognizeLinkedEntities(text);
        LinkedEntityCollection linkedEntities = linkedResponse.Value;
        foreach (LinkedEntity entity in linkedEntities)
        {
            yield return $"Linked Entity detected: '{entity.Name}' with Url: {entity.Url}";
        }

        // Detect PII
        Response<PiiEntityCollection> piiResponse = _textClient.RecognizePiiEntities(text);
        PiiEntityCollection piiEntities = piiResponse.Value;
        if (piiEntities.Any())
        {
            foreach (PiiEntity entity in piiEntities)
            {
                string category = entity.Category.ToString();
                if (!string.IsNullOrWhiteSpace(entity.SubCategory))
                {
                    category += "/" + entity.SubCategory;
                }

                yield return $"PII Encountered '{entity.Text}' (Category: {category}) with {entity.ConfidenceScore:P} confidence";
            }
            yield return $"Redacted Text: {piiEntities.RedactedText}";
        }

        // Detect Sentiment
        Response<DocumentSentiment> sentimentResponse = _textClient.AnalyzeSentiment(text);
        DocumentSentiment sentiment = sentimentResponse.Value;
        SentimentConfidenceScores confidence = sentiment.ConfidenceScores;
        yield return $"Detected Sentiment {sentiment.Sentiment} with positive / neutral / negative confidence scores of {sentiment.ConfidenceScores.Positive:P1}, {confidence.Neutral:P1}, {sentiment.ConfidenceScores.Negative:P1}";
    }

}
