using Azure.AI.TextAnalytics;
using Azure;

namespace MattEland.AutomatingMyDog.Core;

public class TextAnalyticsHelper {
    private readonly TextAnalyticsClient _textClient;

    public TextAnalyticsHelper(string subscriptionKey, string endpoint) {
        if (string.IsNullOrWhiteSpace(subscriptionKey)) {
            throw new ArgumentException($"'{nameof(subscriptionKey)}' cannot be null or whitespace.", nameof(subscriptionKey));
        }

        if (string.IsNullOrWhiteSpace(endpoint)) {
            throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or whitespace.", nameof(endpoint));
        }

        _textClient = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(subscriptionKey));
    }

    public IEnumerable<AppMessage> AnalyzeText(string text) {
        List<AppMessage> messages = new();

        try {
            const MessageSource source = MessageSource.TextAnalytics;

            // Detect Language
            Response<DetectedLanguage> langResponse = _textClient.DetectLanguage(text);
            DetectedLanguage language = langResponse.Value;
            messages.Add(new AppMessage(language.Name, source) { 
                Items = new List<string> { 
                    $"ISO Code {langResponse.Value.Iso6391Name}", 
                    $"{langResponse.Value.ConfidenceScore:P0} Confidence" } 
            });

            // Detect Key Phrases
            Response<KeyPhraseCollection> keyPhrasesResponse = _textClient.ExtractKeyPhrases(text);
            KeyPhraseCollection keyPhrases = keyPhrasesResponse.Value;
            if (keyPhrases.Any()) {
                messages.Add(new AppMessage("Key Phrases", source) { Items = keyPhrases.ToList() });
            }

            // Detect Entities
            Response<CategorizedEntityCollection> recognizeResponse = _textClient.RecognizeEntities(text);
            CategorizedEntityCollection entities = recognizeResponse.Value;
            if (entities.Any()) {
                foreach (CategorizedEntity entity in entities) {
                    messages.Add(new AppMessage($"Detected entity '{entity.Text}' (Category: {entity.Category}) with {entity.ConfidenceScore:P0} confidence", source));
                }
            }

            // Detect Linked Entities
            Response<LinkedEntityCollection> linkedResponse = _textClient.RecognizeLinkedEntities(text);
            LinkedEntityCollection linkedEntities = linkedResponse.Value;
            foreach (LinkedEntity entity in linkedEntities) {
                messages.Add(new AppMessage($"Linked Entity detected: '{entity.Name}' with Url: {entity.Url}", source));
            }

            // Detect PII
            Response<PiiEntityCollection> piiResponse = _textClient.RecognizePiiEntities(text);
            PiiEntityCollection piiEntities = piiResponse.Value;
            if (piiEntities.Any()) {
                foreach (PiiEntity entity in piiEntities) {
                    string category = entity.Category.ToString();
                    if (!string.IsNullOrWhiteSpace(entity.SubCategory)) {
                        category += "/" + entity.SubCategory;
                    }

                    messages.Add(new AppMessage($"PII Encountered '{entity.Text}' (Category: {category}) with {entity.ConfidenceScore:P0} confidence", source));
                }
                messages.Add(new AppMessage($"Redacted Text: {piiEntities.RedactedText}", source));
            }

            // Detect Sentiment
            Response<DocumentSentiment> sentimentResponse = _textClient.AnalyzeSentiment(text);
            DocumentSentiment sentiment = sentimentResponse.Value;
            SentimentConfidenceScores confidence = sentiment.ConfidenceScores;
            messages.Add(new AppMessage($"Sentiment {sentiment.Sentiment}", source) {
                Items = new List<string> {
                $"Positive: {sentiment.ConfidenceScores.Positive:P0}",
                $"Neutral: {sentiment.ConfidenceScores.Neutral:P0}",
                $"Negative: {sentiment.ConfidenceScores.Negative:P0}" }
            });
        }
        catch (AggregateException ex) {
            Exception? innerEx = ex.InnerException;

            if (innerEx != null) {
                string errorMessage = $"Could not process text: {innerEx.Message}";
                messages.Add(new AppMessage(errorMessage, MessageSource.Error) { SpeakText = errorMessage });
            }
        }

        return messages;
    }

}
