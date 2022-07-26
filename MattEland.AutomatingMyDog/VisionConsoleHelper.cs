using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace MattEland.AutomatingMyDog;

public class VisionConsoleHelper
{

    public void DisplayVisionCaptions(ImageAnalysis imageAnalysis)
    {
        Console.WriteLine("\r\nDescription: ");
        foreach (ImageCaption? caption in imageAnalysis.Description.Captions)
        {
            if (caption == null) continue;

            Console.WriteLine($"{caption.Text} (Confidence: {caption.Confidence:p})");
        }
    }
    
    public void AddAndDisplayVisionCategories(ImageAnalysis imageAnalysis, List<string>? items = null)
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

    public void AddAndDisplayVisionObjects(ImageAnalysis imageAnalysis, List<string>? items = null)
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

    public void AddAndDisplayVisionTags(ImageAnalysis imageAnalysis, List<string>? items = null)
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