// See https://aka.ms/new-console-template for more information

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

ComputerVisionClient computerVision = new(new ApiKeyServiceClientCredentials(key));
computerVision.Endpoint = endpoint;

string imagePath = "images/DogCouchNap.jpg";

if (!File.Exists(imagePath))
{
    Console.WriteLine("\nUnable to open or read local image path:\n{0} \n", imagePath);
    return;
}
Console.WriteLine($"Analyzing {imagePath}...");

IList<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>
{
    VisualFeatureTypes.Categories,
    VisualFeatureTypes.Description,
    VisualFeatureTypes.ImageType,
    VisualFeatureTypes.Tags,
    VisualFeatureTypes.Objects
};

ImageAnalysis analysis;
using (Stream imageStream = File.OpenRead(imagePath))
{
    analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
}

VisionConsoleHelper visionHelper = new();

visionHelper.DisplayVisionCaptions(analysis);
visionHelper.DisplayVisionObjects(analysis);
visionHelper.DisplayVisionTags(analysis);
