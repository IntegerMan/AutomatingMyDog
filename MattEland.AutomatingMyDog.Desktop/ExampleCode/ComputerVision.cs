using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

public async Task AnalyzeImage(string filePath) {

    // These should come from a config file
    string key = "123abc45def67g89h0i12345jk6lmno7";
    string endpoint = "https://yourendpoint.cognitiveservices.azure.com/";

    // Authenticate with a Azure Cognitive Services or a Computer Vision resource
    ComputerVisionClient client = new(key);
    computerVision.Endpoint = endpoint;


    
    
    // Read the source image
    await using Stream imageStream = File.OpenRead(filePath);

    

    
    // We need to tell it what types of results we care about
    List<VisualFeatureTypes?> features = new()
    {
        VisualFeatureTypes.Categories,
        VisualFeatureTypes.Description,
        VisualFeatureTypes.Tags,
        VisualFeatureTypes.Objects,
        VisualFeatureTypes.Adult,
        VisualFeatureTypes.Brands,
        VisualFeatureTypes.Color,
    };

    // Perform the image analysis
    var imageAnalysis = await client.AnalyzeImageInStreamAsync(imageStream, features);

    
    
    // The result contains all sorts of interesting things...

    // Captions
    Console.WriteLine("\r\nDescription: ");
    foreach (ImageCaption caption in imageAnalysis.Description.Captions) {
        Console.WriteLine($"{caption.Text} (Confidence: {caption.Confidence:p})");
    }


    
    // Categories
    Console.WriteLine("\r\nCategories: ");
    foreach (Category category in imageAnalysis.Categories) {
        Console.WriteLine($"{category.Name} (Confidence: {category.Score:p})");
    }


    
    // Tags
    Console.WriteLine("\r\nTags:");
    foreach (ImageTag tag in imageAnalysis.Tags) {
        Console.WriteLine($"{tag.Name} (Confidence: {tag.Confidence:p})");
    }


    
    // Object Detection
    Console.WriteLine("\r\nObjects:");
    foreach (DetectedObject obj in imageAnalysis.Objects) {
        BoundingRect rect = obj.Rectangle;

        string bounding = $"({rect.X},{rect.Y}):" + 
            $"({rect.X + rect.W},{rect.Y + rect.H})";

        Console.WriteLine($"{obj.ObjectProperty} " +
            $"(Confidence: {obj.Confidence:p}) at {bounding}");
    }

    

    // Color Info
    Console.WriteLine($"Accent color #{imageAnalysis.Color.AccentColor}")
    foreach (string color in imageAnalysis.Color.DominantColors) {
        Console.WriteLine($"Other color: {color}");
    }


    
    // Content Moderation
    Console.WriteLine($"Adult Score: {imageAnalysis.Adult.AdultScore:P}");
    Console.WriteLine($"Racy Score: {imageAnalysis.Adult.RacyScore:P}");
    Console.WriteLine($"Gory Score: {imageAnalysis.Adult.GoreScore:P}");


    
    // Brand Detection
    foreach (var brand in  imageAnalysis.Brands) {
        Console.WriteLine($"Brand: {brand.Name}");
    }

    
}