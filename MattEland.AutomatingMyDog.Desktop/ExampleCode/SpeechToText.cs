using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

public async Task<string> ListenToSpokenWords() {

    // Configure your subscription and region
    // Read these from a config file in production
    string region = "northcentralus";
    string subscriptionKey = "123abc45def67g89h0i12345jk6lmno7";
    SpeechConfig config = SpeechConfig.FromSubscription(subscriptionKey, region);

    // Listen to speech
    SpeechRecognitionResult result;
    using (SpeechRecognizer recognizer = new(config)) {
        result = await recognizer.RecognizeOnceAsync();
    }

    // Handle the result
    switch (result.Reason) {
        case ResultReason.RecognizedSpeech:
            return result.Text; // your words in string form

        case ResultReason.NoMatch:
            return "Could not hear any audio. Your mic may not be working.";

        case ResultReason.Canceled:
            return "Recognition canceled. Your config settings may be wrong.";

        default:
            return $"Unhandled recognition result: {result.Reason}";
    }
}
