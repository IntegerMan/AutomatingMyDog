using Microsoft.CognitiveServices.Speech;

namespace MattEland.AutomatingMyDog;

public class DemoSpeechHelper
{
    private readonly SpeechConfig _speechConfig;

    public DemoSpeechHelper(string subscriptionKey, string region, string voice = "en-US-GuyNeural")
    {
        SpeechConfig speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        speechConfig.SpeechSynthesisVoiceName = voice;

        _speechConfig = speechConfig;
    }

    public void SayMessage(string message) => SayMessageAsync(message);

    public async Task SayMessageAsync(string message)
    {
        Console.WriteLine();
        Console.WriteLine($"\"{message}\"");
        Console.WriteLine();

        using SpeechSynthesizer synthesizer = new(_speechConfig);
        using SpeechSynthesisResult? result = await synthesizer.SpeakTextAsync(message);
    }

    public string? ListenToSpokenText()
    {
        // Listen to a speech stream and transcribe it to words
        using (SpeechRecognizer recognizer = new(_speechConfig))
        {
            SpeechRecognitionResult? result = recognizer.RecognizeOnceAsync().Result;

            switch (result.Reason)
            {
                case ResultReason.Canceled:
                    Console.WriteLine("Speech Recognition canceled.");
                    return null;

                case ResultReason.NoMatch:
                    Console.WriteLine("Speech Recognition could not understand audio. Your mic may not be working.");
                    return null;

                case ResultReason.RecognizedSpeech:
                    return result.Text;

                default:
                    Console.WriteLine("Unhandled speech recognition result: " + result.Reason);
                    return null;
            }
        }
    }

}