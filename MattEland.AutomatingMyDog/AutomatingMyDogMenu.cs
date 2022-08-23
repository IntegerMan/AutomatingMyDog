using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Rest;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials;

namespace MattEland.AutomatingMyDog;

public class AutomatingMyDogMenu
{
    private readonly SpeechConfig _speechConfig;
    private readonly ComputerVisionClient _computerVision;
    private readonly Guid _luisAppId;
    private readonly LUISRuntimeClient _luisClient;
    private readonly TextAnalyticsClient _textClient;
    private readonly string _luisSlotId;

    public AutomatingMyDogMenu(AutomatingMyDogConfig configData)
    {
        // Set up speech synthesis
        _speechConfig = SpeechConfig.FromSubscription(configData.Key, configData.Region);
        _speechConfig.SpeechSynthesisVoiceName = "en-US-GuyNeural";

        // Set up computer vision
        ApiKeyServiceClientCredentials visionCredentials = new(configData.Key);
        _computerVision = new ComputerVisionClient(visionCredentials);
        _computerVision.Endpoint = configData.Endpoint;

        // Set up text analytics client
        _textClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(configData.Key));
        _textClient.Endpoint = configData.Endpoint;

        // Set up Language Understanding (LUIS)
        _luisClient = new LUISRuntimeClient(new ApiKeyServiceClientCredentials(configData.Key));
        _luisClient.Endpoint = configData.Endpoint;
        _luisSlotId = configData.SlotId; // Determine if we're looking at the production or staging slot
        _luisAppId = new Guid(configData.AppId);
        
    }

    public void ShowMainMenu()
    {
        SayMessageFireAndForget("Hello there!");

        Console.WriteLine("Welcome to 'Automating my Dog' by Matt Eland (@IntegerMan).");

        bool stillGoing = true;
        do
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to do?");
            Console.WriteLine();
            Console.WriteLine("1) Look at pictures (and bark if anything is interesting)");
            Console.WriteLine("2) Listen to commands (and probably ignore them)");
            Console.WriteLine("3) React to typed commands (and probably ignore them)");
            Console.WriteLine("Q) Quit");
            Console.WriteLine();
            string option = Console.ReadLine()!;
            Console.WriteLine();

            switch (option.ToUpperInvariant())
            {
                case "1": // Computer Vision / Speech Synthesis
                    LookAtPicturesAsync().RunSynchronously();
                    break;

                case "2": // Speech Recognition / LUIS
                    ListenToSpeechAsync();
                    break;

                case "3": // LUIS
                    PromptForCommandAsync().RunSynchronously();
                    break;

                case "Q": // Quit
                    stillGoing = false;
                    SayMessageFireAndForget("Goodbye, friend!");
                    break;

                default:
                    SayMessageFireAndForget("I don't understand.");
                    break;
            }
        } while (stillGoing);
    }

    private void SayMessageFireAndForget(string message) => SayMessageAsync(message);

    private async Task SayMessageAsync(string message)
    {
        Console.WriteLine();
        Console.WriteLine($"\"{message}\"");
        Console.WriteLine();

        using SpeechSynthesizer synthesizer = new(_speechConfig);
        using SpeechSynthesisResult? result = await synthesizer.SpeakTextAsync(message);
    }

    private async Task LookAtPicturesAsync()
    {

        string[] files = Directory.GetFiles("images");

        DemoImageAnalyzer analyzer = new();
        foreach (string imagePath in files.Skip(3).Take(1))
        {
            List<string> detectedItems = await analyzer.AnalyzeImageAsync(imagePath, _computerVision);

            // Potentially bark at the thing we saw
            bool ShouldBarkAt(string thing)
            {
                thing = thing.ToLowerInvariant();

                return thing.Contains("squirrel") ||
                       thing.Contains("rabbit") ||
                       thing.Contains("rodent") ||
                       thing.Contains("dog");
            }

            string? barkTarget = detectedItems.FirstOrDefault(ShouldBarkAt);
            if (barkTarget != null)
            {
                string message = $"I saw a {barkTarget}; Bark, bark, bark!";

                SayMessageFireAndForget(message);
            }
        }
    }

    private async Task ListenToSpeechAsync()
    {
        // Listen to a speech stream and transcribe it to words
        using (SpeechRecognizer recognizer = new(_speechConfig))
        {
            SpeechRecognitionResult? result = await recognizer.RecognizeOnceAsync();

            switch (result.Reason)
            {
                case ResultReason.Canceled:
                    Console.WriteLine("Speech Recognition canceled.");
                    break;

                case ResultReason.NoMatch:
                    Console.WriteLine("Speech Recognition could not understand audio.");
                    break;

                case ResultReason.RecognizedSpeech:
                    await ProcessCommandAsync(result.Text);
                    break;
            }
        }
    }

    private async Task PromptForCommandAsync()
    {
        // TODO: Listen to a speech stream and transcribe it to words
        Console.WriteLine("What would you like to tell me?");
        string input = Console.ReadLine()!;

        await ProcessCommandAsync(input);
    }

    private async Task ProcessCommandAsync(string command)
    {
        // Protect against empty inputs; don't display any errors to the user, though
        if (string.IsNullOrWhiteSpace(command))
        {
            return;
        }

        // Display the words for validation
        Console.WriteLine($"You said: \"{command}\"");
        Console.WriteLine();

        // Call out to text analytics and try to understand the parts of speech
        await AnalyzeTextAsync(command);

        // Parse the text into an intent
        string intent = await DetectIntentAsync(command);
        switch (intent.ToUpperInvariant())

        {
            case "GOOD BOY":
                SayMessageFireAndForget("Jester is a good doggo!");
                break;

            case "WALK":
                SayMessageFireAndForget("Why yes, Jester DOES want to go on a walk!");
                break;

            default:
                SayMessageFireAndForget("I don't understand.");
                break;
        }
    }

    private async Task AnalyzeTextAsync(string text)
    {
        // Detect Language
        LanguageResult? langResult = await _textClient.DetectLanguageAsync(text, countryHint: "US");
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
        KeyPhraseResult result = await _textClient.KeyPhrasesAsync(text, language: languageCode);
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
        EntitiesResult entities = await _textClient.EntitiesAsync(text, language: languageCode);
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
        SentimentResult sentimentResult = await _textClient.SentimentAsync(text, language: languageCode);
        if (sentimentResult.Score.HasValue)
        {
            Console.WriteLine();
            Console.WriteLine($"Sentiment: {sentimentResult.Score}");
        }
    }

    private async Task<string> DetectIntentAsync(string message)
    {
        PredictionRequest request = new() { Query = message };
        HttpOperationResponse<PredictionResponse> predictResult =
            await _luisClient.Prediction.GetSlotPredictionWithHttpMessagesAsync(_luisAppId, _luisSlotId, request);

        Prediction prediction = predictResult.Body.Prediction;

        Console.WriteLine();
        Console.WriteLine("Intents: ");
        foreach (var intent in prediction.Intents)
        {
            Console.WriteLine(intent.Key + ": " + intent.Value.Score);
        }

        string topIntent = prediction.TopIntent;

        // Ensure we only react to intents over a specific threshold
        if (prediction.Intents.Max(i => i.Value.Score) < 0.6)
        {
            topIntent = "Unknown";
        }

        Console.WriteLine();
        Console.WriteLine("Top intent was: " + topIntent);

        return topIntent;
    }

}