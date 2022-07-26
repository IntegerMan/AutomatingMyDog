using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace MattEland.AutomatingMyDog;

public class DemoImageAnalyzer
{
    public async Task<List<string>> AnalyzeImageAsync(string filePath, IComputerVisionClient computerVisionClient)
    {
        List<string> items = new();
        
        Console.WriteLine($"Analyzing {filePath}...");

        List<VisualFeatureTypes?> features = new()
        {
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Description,
            VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Objects
        };

        await using Stream imageStream = File.OpenRead(filePath);
        
        ImageAnalysis analysis = await computerVisionClient.AnalyzeImageInStreamAsync(imageStream, features);

        VisionConsoleHelper visionHelper = new();
        visionHelper.DisplayVisionCaptions(analysis);
        visionHelper.AddAndDisplayVisionTags(analysis, items);
        visionHelper.AddAndDisplayVisionObjects(analysis, items);
        visionHelper.AddAndDisplayVisionCategories(analysis, items);

        return items;
    }
}