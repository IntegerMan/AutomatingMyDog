using MattEland.AutomatingMyDog;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

ConfigurationManager configManager = new();
ConfigData data = configManager.LoadConfigData();

ApiKeyServiceClientCredentials credentials = new(data.Key);

ComputerVisionClient computerVision = new(credentials);
computerVision.Endpoint = data.Endpoint;

string[] files = Directory.GetFiles("images");

foreach (string imagePath in files)
{
    DemoImageAnalyzer analyzer = new();
    ImageAnalysis analysis = await analyzer.AnalyzeImageAsync(imagePath, computerVision);
}

