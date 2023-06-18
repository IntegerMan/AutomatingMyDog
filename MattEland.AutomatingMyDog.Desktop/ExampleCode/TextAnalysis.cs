using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

public async Task<bool> SayHelloAsync() {

    // Read these from a config file in production
    string region = "northcentralus";
    string subscriptionKey = "123abc45def67g89h0i12345jk6lmno7";

    // Configure your subscription and region
    SpeechConfig config = SpeechConfig.FromSubscription(subscriptionKey, region);

    // Set up the Synthesizer to use the voice we want
    config.SpeechSynthesisVoiceName = "en-US-GuyNeural";
    using SpeechSynthesizer synthesizer = new(config);

    // Actually say the message
    string message = "Welcome to DogOS";
    using SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(message);

    // If we're correctly configured this should give us a completed result
    bool succeeded = result.Reason == ResultReason.SynthesizingAudioCompleted;

    // result.AudioData will have the raw WAV file if you want to save it
    byte[] wavData = result.AudioData;

    return succeeded;
}
