using Microsoft.CognitiveServices.Speech;

namespace MattEland.AutomatingMyDog.Core;

public class SpeechHelper
{
    private readonly SpeechConfig _speechConfig;

    public SpeechHelper(string subscriptionKey, string region, string voiceName= "en-US-GuyNeural")
    {
        if (string.IsNullOrWhiteSpace(subscriptionKey))
        {
            throw new ArgumentException($"'{nameof(subscriptionKey)}' cannot be null or whitespace.", nameof(subscriptionKey));
        }

        if (string.IsNullOrWhiteSpace(region))
        {
            throw new ArgumentException($"'{nameof(region)}' cannot be null or whitespace.", nameof(region));
        }

        if (string.IsNullOrWhiteSpace(voiceName))
        {
            throw new ArgumentException($"'{nameof(voiceName)}' cannot be null or whitespace.", nameof(voiceName));
        }

        VoiceName = voiceName;

        _speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        _speechConfig.SpeechSynthesisVoiceName = voiceName;
    }

    public string VoiceName { get; set; } = "en-US-GuyNeural";

    public void SayMessage(string message) => SayMessageAsync(message);

    public async Task SayMessageAsync(string message)
    {
        _speechConfig.SpeechSynthesisVoiceName = VoiceName;

        using SpeechSynthesizer synthesizer = new(_speechConfig);
        using SpeechSynthesisResult? result = await synthesizer.SpeakTextAsync(message);
    }

    public string ListenToSpokenText()
    {
        // Listen to a speech stream and transcribe it to words
        using (SpeechRecognizer recognizer = new(_speechConfig))
        {
            SpeechRecognitionResult? result = recognizer.RecognizeOnceAsync().Result;

            return result.Reason switch
            {
                ResultReason.RecognizedSpeech => result.Text,
                ResultReason.Canceled => throw new SpeechException("Speech Recognition canceled.", result),
                ResultReason.NoMatch => throw new SpeechException("Speech Recognition could not understand audio. Your mic may not be working.", result),
                _ => throw new SpeechException($"Unhandled speech recognition result: {result.Reason}", result),
            };
        }
    }

}
