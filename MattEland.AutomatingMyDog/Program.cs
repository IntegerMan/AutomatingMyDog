using MattEland.AutomatingMyDog;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

string key;
string endpoint;

using (StreamReader file = File.OpenText("credentials.json"))
using (JsonTextReader reader = new(file))
{
    JObject jObj = (JObject)JToken.ReadFrom(reader);
    key = jObj["key"].Value<string>();
    endpoint = jObj["endpoint"].Value<string>();
}

ApiKeyServiceClientCredentials credentials = new(key);

ComputerVisionClient computerVision = new(credentials);
computerVision.Endpoint = endpoint;

string[] files = Directory.GetFiles("images");

foreach (string imagePath in files)
{
    DemoImageAnalyzer analyzer = new();
    ImageAnalysis analysis = await analyzer.AnalyzeImageAsync(imagePath, computerVision);
}

