using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MattEland.AutomatingMyDog.Core;

public class TextToSpeechHelper
{
    private readonly SpeechConfig _speechConfig;

    public TextToSpeechHelper(string subscriptionKey, string region, string voiceName= "en-US-GuyNeural")
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

        _speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        _speechConfig.SpeechSynthesisVoiceName = voiceName;
    }

    public void SayMessage(string message) => SayMessageAsync(message);

    public async Task SayMessageAsync(string message)
    {
        using SpeechSynthesizer synthesizer = new(_speechConfig);
        using SpeechSynthesisResult? result = await synthesizer.SpeakTextAsync(message);
    }

}
