using MattEland.AutomatingMyDog;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.CognitiveServices.Speech;

ConfigurationManager configManager = new();
ConfigData configData = configManager.LoadConfigData();

await ShowMainMenuAsync();

async Task ShowMainMenuAsync()
{
    Console.WriteLine("Welcome to 'Automating my Dog' by Matt Eland (@IntegerMan).");
    
    bool stillGoing = true;
    do
    {
        Console.WriteLine();
        Console.WriteLine("What would you like to do?");
        Console.WriteLine();
        Console.WriteLine("1) Look at pictures (and bark if anything is interesting)");
        Console.WriteLine("2) Listen to commands (and probably ignore them)");
        Console.WriteLine("Q) Quit");
        Console.WriteLine();
        string option = Console.ReadLine()!;
        Console.WriteLine();

        switch (option.ToUpperInvariant())
        {
            case "1": // Computer Vision / Speech Synthesis
                await LookAtPicturesAsync();
                break;

            case "2": // Speech Recognition / LUIS
                await ListenToSpeechAsync();
                break;
            
            case "Q": // Quit
                stillGoing = false;
                await SayMessageAsync("Goodbye, friend!");
                break;

            default:
                await SayMessageAsync("I don't understand.");
                break;
        }
    } while (stillGoing);
}

async Task LookAtPicturesAsync()
{
    ApiKeyServiceClientCredentials credentials = new(configData.Key);

    ComputerVisionClient computerVision = new(credentials);
    computerVision.Endpoint = configData.Endpoint;

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
            string message = $"I saw a {barkTarget}; Bark, bark, bark!";

            await SayMessageAsync(message);
        }
    }
}

async Task ListenToSpeechAsync()
{
    // TODO: Listen to a speech stream and transcribe it to words

    // TODO: Display the words for validation
    
    // TODO: Call out to text analytics and try to understand the parts of speech

    // TODO: Parse the text into an intent

    // Speech synthesize an "Arf" if the intent matches "speak"
    await SayMessageAsync("Arf, arf!");
}

async Task SayMessageAsync(string message)
{
    SpeechConfig speechConfig = SpeechConfig.FromSubscription(configData.Key, configData.Region);
    speechConfig.SpeechSynthesisVoiceName = "en-US-GuyNeural";
    
    using SpeechSynthesizer synthesizer = new(speechConfig);
    using SpeechSynthesisResult? result = await synthesizer.SpeakTextAsync(message);
}


