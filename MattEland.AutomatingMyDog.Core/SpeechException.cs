using Microsoft.CognitiveServices.Speech;

namespace MattEland.AutomatingMyDog.Core;

public class SpeechException : Exception
{
    public SpeechException(string message, SpeechRecognitionResult? result = null) : base(message)
    {
        Result = result;
    }

    public SpeechRecognitionResult? Result { get; }
}
