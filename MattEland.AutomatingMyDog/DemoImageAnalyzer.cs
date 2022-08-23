using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace MattEland.AutomatingMyDog;

public class DemoImageAnalyzer
{
    private readonly ComputerVisionClient _computerVision;

    public DemoImageAnalyzer(string subscriptionKey, string endpoint)
    {
        ApiKeyServiceClientCredentials visionCredentials = new(subscriptionKey);
        _computerVision = new ComputerVisionClient(visionCredentials);
        _computerVision.Endpoint = endpoint;

    }

    public async Task<List<string>> AnalyzeImageAsync(string filePath)
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
        
        ImageAnalysis analysis = await _computerVision.AnalyzeImageInStreamAsync(imageStream, features);

        DisplayVisionCaptions(analysis);
        AddAndDisplayVisionTags(analysis, items);
        AddAndDisplayVisionObjects(analysis, items);
        AddAndDisplayVisionCategories(analysis, items);

        return items;
    }

    private void DisplayVisionCaptions(ImageAnalysis imageAnalysis)
    {
        Console.WriteLine("\r\nDescription: ");
        foreach (ImageCaption? caption in imageAnalysis.Description.Captions)
        {
            if (caption == null) continue;

            Console.WriteLine($"{caption.Text} (Confidence: {caption.Confidence:p})");
        }
    }

    private void AddAndDisplayVisionCategories(ImageAnalysis imageAnalysis, List<string>? items = null)
    {
        if (!imageAnalysis.Categories.Any()) return;

        Console.WriteLine("\r\nCategories: ");
        foreach (Category? category in imageAnalysis.Categories)
        {
            if (category == null) continue;

            items?.Add(category.Name);

            Console.WriteLine($"{category.Name} (Confidence: {category.Score:p})");
        }
    }

    private void AddAndDisplayVisionObjects(ImageAnalysis imageAnalysis, List<string>? items = null)
    {
        if (!imageAnalysis.Objects.Any()) return;

        Console.WriteLine("\r\nObjects:");
        foreach (DetectedObject? obj in imageAnalysis.Objects)
        {
            if (obj == null) continue;

            items?.Add(obj.ObjectProperty);

            Console.WriteLine($"{obj.ObjectProperty} (Confidence: {obj.Confidence:p}) at ({obj.Rectangle.X},{obj.Rectangle.Y}):({obj.Rectangle.X + obj.Rectangle.W},{obj.Rectangle.Y + obj.Rectangle.H})");
            ObjectHierarchy? parent = obj.Parent;
            while (parent != null)
            {
                Console.WriteLine($"Parent: {parent.ObjectProperty} (Confidence: {parent.Confidence:p})");

                items?.Add(parent.ObjectProperty);
                parent = parent.Parent;
            }
        }
    }

    private void AddAndDisplayVisionTags(ImageAnalysis imageAnalysis, List<string>? items = null)
    {
        if (!imageAnalysis.Tags.Any()) return;

        Console.WriteLine("\r\nTags:");
        foreach (ImageTag? tag in imageAnalysis.Tags)
        {
            if (tag == null) continue;

            items?.Add(tag.Name);

            Console.WriteLine($"{tag.Name} (Confidence: {tag.Confidence:p})");
        }
    }
}