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

    public async Task<List<string>> DetectItemsAsync(string filePath)
    {
        Console.WriteLine($"Analyzing {filePath}...");

        // We need to tell it what types of results we care about
        List<VisualFeatureTypes?> features = new()
        {
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Description,
            VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Objects
        };

        // Call out to Computer Vision to get the results
        await using Stream imageStream = File.OpenRead(filePath);
        ImageAnalysis analysis = await _computerVision.AnalyzeImageInStreamAsync(imageStream, features);

        // Interpret the results
        IEnumerable<string> captions = ListVisionCaptions(analysis);
        IEnumerable<string> detectedTags = ListVisionTags(analysis);
        IEnumerable<string> detectedCategories = ListVisionCategories(analysis);
        IEnumerable<string> detectedObjects = ListVisionObjects(analysis);

        InputHelper.PressAnyKey();

        // Merge everything into a collection
        List<string> results = new(captions);
        results.AddRange(detectedObjects);
        results.AddRange(detectedTags);
        
        return results;
    }

    private List<string> ListVisionCaptions(ImageAnalysis imageAnalysis)
    {
        List<string> captions = new();
        
        Console.WriteLine("\r\nDescription: ");
        foreach (ImageCaption caption in imageAnalysis.Description.Captions)
        {
            Console.WriteLine($"{caption.Text} (Confidence: {caption.Confidence:p})");

            captions.Add(caption.Text);
        }

        return captions;
    }

    private IEnumerable<string> ListVisionCategories(ImageAnalysis imageAnalysis)
    {
        List<string> categories = new();
        
        if (imageAnalysis.Categories.Any())
        {
            Console.WriteLine("\r\nCategories: ");
            foreach (Category category in imageAnalysis.Categories)
            {
                Console.WriteLine($"{category.Name} (Confidence: {category.Score:p})");

                categories.Add(category.Name);
            }
        }

        return categories;
    }

    private IEnumerable<string> ListVisionObjects(ImageAnalysis imageAnalysis)
    {
        List<string> items = new();

        if (imageAnalysis.Objects.Any())
        {

            Console.WriteLine("\r\nObjects:");
            foreach (DetectedObject obj in imageAnalysis.Objects)
            {
                items.Add(obj.ObjectProperty);

                BoundingRect rect = obj.Rectangle;
                string bounding = $"({rect.X},{rect.Y}):({rect.X + rect.W},{rect.Y + rect.H})";
                Console.WriteLine($"{obj.ObjectProperty} (Confidence: {obj.Confidence:p}) at {bounding}");
                
                /* If we want parents and their parents, this would work:
                ObjectHierarchy? parent = obj.Parent;
                while (parent != null)
                {
                    Console.WriteLine($"\tParent: {parent.ObjectProperty} (Confidence: {parent.Confidence:p})");

                    items.Add(parent.ObjectProperty);
                    parent = parent.Parent;
                }
                */
            }
        }

        return items;
    }

    private IEnumerable<string> ListVisionTags(ImageAnalysis imageAnalysis)
    {
        List<string> items = new();
        
        if (imageAnalysis.Tags.Any())
        {
            Console.WriteLine("\r\nTags:");
            foreach (ImageTag tag in imageAnalysis.Tags)
            {
                items.Add(tag.Name);

                Console.WriteLine($"{tag.Name} (Confidence: {tag.Confidence:p})");
            }
        }

        return items;
    }
}