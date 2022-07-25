using MattEland.AutomatingMyDog;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

ConfigurationManager configManager = new();
ConfigData data = configManager.LoadConfigData();

ApiKeyServiceClientCredentials credentials = new(data.Key);

ComputerVisionClient computerVision = new(credentials);
computerVision.Endpoint = data.Endpoint;

string[] files = Directory.GetFiles("images");

DemoImageAnalyzer analyzer = new();
foreach (string imagePath in files)
{
    ImageAnalysis analysis = await analyzer.AnalyzeImageAsync(imagePath, computerVision);
}

// TODO: Speech synthesize a "Bark" if an animal is seen outdoors

// TODO: Listen to a speech stream and transcribe it to words

// TODO: Parse the speech file into an intent

// TODO: Speech synthesize an "Arf" if the intent matches "speak"