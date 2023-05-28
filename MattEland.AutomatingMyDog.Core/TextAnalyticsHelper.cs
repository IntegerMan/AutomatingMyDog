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

    public IEnumerable<AppMessage> AnalyzeText(string text)
    {
        const MessageSource source = MessageSource.TextAnalytics;

        // Detect Language
        Response<DetectedLanguage> langResponse = _textClient.DetectLanguage(text);
        DetectedLanguage language = langResponse.Value;
        yield return new AppMessage($"Detected Language {language.Name} ({langResponse.Value.Iso6391Name}) with confidence of {langResponse.Value.ConfidenceScore:P0}", source);

        // Detect Key Phrases
        Response<KeyPhraseCollection> keyPhrasesResponse = _textClient.ExtractKeyPhrases(text);
        KeyPhraseCollection keyPhrases = keyPhrasesResponse.Value;
        yield return new AppMessage($"Key Phrases: {string.Join(", ", keyPhrases)}", source);

        // Detect Entities
        Response<CategorizedEntityCollection> recognizeResponse = _textClient.RecognizeEntities(text);
        CategorizedEntityCollection entities = recognizeResponse.Value;
        if (entities.Any())
        {
            foreach (CategorizedEntity entity in entities)
            {
                yield return new AppMessage($"Detected entity '{entity.Text}' (Category: {entity.Category}) with {entity.ConfidenceScore:P0} confidence", source);
            }
        }

        // Detect Linked Entities
        Response<LinkedEntityCollection> linkedResponse = _textClient.RecognizeLinkedEntities(text);
        LinkedEntityCollection linkedEntities = linkedResponse.Value;
        foreach (LinkedEntity entity in linkedEntities)
        {
            yield return new AppMessage($"Linked Entity detected: '{entity.Name}' with Url: {entity.Url}", source);
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

                yield return new AppMessage($"PII Encountered '{entity.Text}' (Category: {category}) with {entity.ConfidenceScore:P0} confidence", source);
            }
            yield return new AppMessage($"Redacted Text: {piiEntities.RedactedText}", source);
        }

        // Detect Sentiment
        Response<DocumentSentiment> sentimentResponse = _textClient.AnalyzeSentiment(text);
        DocumentSentiment sentiment = sentimentResponse.Value;
        SentimentConfidenceScores confidence = sentiment.ConfidenceScores;
        yield return new AppMessage($"Detected Sentiment {sentiment.Sentiment} with positive / neutral / negative confidence scores of {sentiment.ConfidenceScores.Positive:P0} / {confidence.Neutral:P0} / {sentiment.ConfidenceScores.Negative:P0}", source);
    }

}
