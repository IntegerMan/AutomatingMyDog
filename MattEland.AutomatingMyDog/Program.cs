using MattEland.AutomatingMyDog;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.CognitiveServices.Speech;

ConfigurationManager configManager = new();
ConfigData configData = configManager.LoadConfigData();

ApiKeyServiceClientCredentials credentials = new(configData.Key);

ComputerVisionClient computerVision = new(credentials);
computerVision.Endpoint = configData.Endpoint;

SpeechConfig config = SpeechConfig.FromSubscription(configData.Key, configData.Region);
config.SpeechSynthesisVoiceName = "en-US-GuyNeural";

string[] files = Directory.GetFiles("images");

DemoImageAnalyzer analyzer = new();
foreach (string imagePath in files.Skip(3).Take(1))
{
    List<string> detectedItems = await analyzer.AnalyzeImageAsync(imagePath, computerVision);

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
        using SpeechSynthesizer synthesizer = new(config);
        using SpeechSynthesisResult? result = await synthesizer.SpeakTextAsync($"I saw a {barkTarget}; Bark, bark, bark!");
    }
}


// TODO: Listen to a speech stream and transcribe it to words

// TODO: Parse the speech file into an intent

// TODO: Speech synthesize an "Arf" if the intent matches "speak"