using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.AI.TextAnalytics;
using Azure;

public async Task<IEnumerable<string>> AnalyzeTextAsync(string text) {

    // Configuration settings - store these in a config file
    string key = "123abc45def67g89h0i12345jk6lmno7";
    string endpoint = "https://yourendpoint.cognitiveservices.azure.com/";
    Uri endpointUri = new(endpoint);

    // Set up our TextAnalyticsClient
    AzureKeyCredential azureCreds = new(key);
    TextAnalyticsClient client = new(endpointUri, azureCreds);



    // Detect Language
    Response<DetectedLanguage> langResponse = client.DetectLanguage(text);
    DetectedLanguage language = langResponse.Value;
    yield return $"Detected Language {language.Name} ({langResponse.Value.Iso6391Name})" +
        $" with confidence of {langResponse.Value.ConfidenceScore:P0}";



    // Detect Key Phrases
    Response<KeyPhraseCollection> keyPhrasesResponse = client.ExtractKeyPhrases(text);
    KeyPhraseCollection keyPhrases = keyPhrasesResponse.Value;
    yield return $"Key Phrases: {string.Join(", ", keyPhrases)}";



    // Detect Entities
    Response<CategorizedEntityCollection> recognizeResponse = client.RecognizeEntities(text);
    CategorizedEntityCollection entities = recognizeResponse.Value;
    foreach (CategorizedEntity entity in entities) {
        yield return $"Detected entity '{entity.Text}' " +
            $"(Category: {entity.Category}) with " +
            $"{entity.ConfidenceScore:P0} confidence";
    }



    // Detect Linked Entities
    Response<LinkedEntityCollection> linkedResponse = client.RecognizeLinkedEntities(text);
    LinkedEntityCollection linkedEntities = linkedResponse.Value;
    foreach (LinkedEntity entity in linkedEntities) {
        yield return $"Linked Entity detected: '{entity.Name}' " +
            $"with Url: {entity.Url}";
    }



    // Detect PII
    Response<PiiEntityCollection> piiResponse = client.RecognizePiiEntities(text);
    PiiEntityCollection piiEntities = piiResponse.Value;
    foreach (PiiEntity entity in piiEntities) {
        string category = entity.Category.ToString();
        if (!string.IsNullOrWhiteSpace(entity.SubCategory)) {
            category += "/" + entity.SubCategory;
        }

        yield return $"PII Encountered '{entity.Text}' " +
            $"(Category: {category}) with " +
            $"{entity.ConfidenceScore:P0} confidence";
    }
    yield return $"Redacted Text: {piiEntities.RedactedText}";



    // Detect Sentiment
    Response<DocumentSentiment> sentimentResponse = client.AnalyzeSentiment(text);
    DocumentSentiment sentiment = sentimentResponse.Value;
    SentimentConfidenceScores confidence = sentiment.ConfidenceScores;
    yield return $"Detected Sentiment {sentiment.Sentiment} with " +
        $"positive / neutral / negative confidence scores of " +
        $"{sentiment.ConfidenceScores.Positive:P0} / " +
        $"{confidence.Neutral:P0} / " +
        $"{sentiment.ConfidenceScores.Negative:P0}";


}
