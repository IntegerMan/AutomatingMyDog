using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace MattEland.AutomatingMyDog;

public class DemoImageAnalyzer
{
    public async Task<ImageAnalysis> AnalyzeImageAsync(string filePath, IComputerVisionClient computerVisionClient)
    {
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
        visionHelper.DisplayVisionObjects(analysis);
        visionHelper.DisplayVisionTags(analysis);
        visionHelper.DisplayVisionCategories(analysis);

        return analysis;
    }
}