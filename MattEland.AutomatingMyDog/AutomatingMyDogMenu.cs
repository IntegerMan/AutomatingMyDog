using MattEland.AutomatingMyDog.Core;

namespace MattEland.AutomatingMyDog;

public class AutomatingMyDogMenu
{
    private readonly DemoImageAnalyzer _visionDemos;
    private readonly DemoSpeechHelper _speechDemos;
    private readonly DemoTextAnalytics _textDemos;
    private readonly DemoLanguageUnderstanding _luisDemos;

    public AutomatingMyDogMenu(AutomatingMyDogConfig configData)
    {
        // Set up Helpers for demo tasks
        _speechDemos = new DemoSpeechHelper(configData.Key, configData.Region);
        _visionDemos = new DemoImageAnalyzer(configData.Key, configData.Endpoint);
        _textDemos = new DemoTextAnalytics(configData.Key, configData.Endpoint);
        _luisDemos = new DemoLanguageUnderstanding(configData.Key, configData.Endpoint, configData.AppId, configData.SlotId);
    }

    public void ShowMainMenu()
    {
        _speechDemos.SayMessage("Hello there!");

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
                    LookAtPictures();
                    break;

                case "2": // Speech Recognition / LUIS
                    string? spokenText = _speechDemos.ListenToSpokenText();

                    RespondToTextCommand(spokenText);
                    break;

                case "3": // LUIS - type in a command
                    Console.WriteLine("What would you like to tell me?");
                    string input = Console.ReadLine()!;

                    RespondToTextCommand(input);
                    break;

                case "Q": // Quit
                    stillGoing = false;
                    _speechDemos.SayMessageAsync("Goodbye, friend!").Wait();
                    break;

                default:
                    _speechDemos.SayMessage("I don't understand.");
                    break;
            }
        } while (stillGoing);
    }

    private void LookAtPictures()
    {
        // Let the user pick which file to analyze
        string? imagePath = @"C:\Dev\repos\AutomatingMyDog\MattEland.AutomatingMyDog.Desktop\bin\Debug\net7.0-windows\snapshot.png";// ImageSelectionHelper.SelectImage(Directory.GetFiles("images"));

        // The user is allowed to not pick an image, in which case we just exit
        if (string.IsNullOrWhiteSpace(imagePath)) return;

        // Have Computer Vision analyze the image            
        List<string> detectedItems = _visionDemos.DetectItemsAsync(imagePath).Result;

        // Potentially bark at the thing we saw
        string? barkTarget = detectedItems.FirstOrDefault(IsSomethingToBarkAt);
        string message;
        if (barkTarget != null)
        {
            // Add the word "a" in front of it if needed, but only if it doesn't start with an article already
            barkTarget = StringHelper.AddArticleIfNotPresent(barkTarget);

            message = $"I saw {barkTarget}; Bark, bark, bark!";
        }
        else
        {
            IEnumerable<string> itemsToMention = detectedItems.Distinct().Take(5);
            message = $"Nothing to bark at, but here's some things I saw: {string.Join(", ", itemsToMention)}";
        }

        _speechDemos.SayMessage(message);
    }

    private static bool IsSomethingToBarkAt(string thing)
    {
        thing = thing.ToLowerInvariant();

        return thing.Contains("squirrel") ||
               thing.Contains("rabbit") ||
               thing.Contains("rodent") ||
               thing.Contains("dog");
    }

    private void RespondToTextCommand(string? command)
    {
        // This can be called with empty / null if the user typed nothing or the mic couldn't get spoken text
        if (string.IsNullOrWhiteSpace(command))
        {
            return;
        }

        // Display the words for validation
        Console.WriteLine($"You said: \"{command}\"");
        Console.WriteLine();

        // Call out to Azure Cognitive Services text analytics and try to understand the parts of speech
        _textDemos.AnalyzeText(command);

        // Use Language Understanding Parse the text into an intent
        string intent = _luisDemos.DetectIntent(command);
        switch (intent.ToUpperInvariant())

        {
            case "GOOD BOY":
                _speechDemos.SayMessage("Jester is a good doggo!");
                break;

            case "WALK":
                _speechDemos.SayMessage("Why yes, Jester DOES want to go on a walk!");
                break;

            default:
                _speechDemos.SayMessage("I don't understand.");
                break;
        }
    }

}