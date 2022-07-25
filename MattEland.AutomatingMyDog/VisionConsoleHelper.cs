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
    
    public void DisplayVisionCategories(ImageAnalysis imageAnalysis)
    {
        if (!imageAnalysis.Categories.Any()) return;

        Console.WriteLine("\r\nCategories: ");
        foreach (Category? category in imageAnalysis.Categories)
        {
            if (category == null) continue;

            Console.WriteLine($"{category.Name} (Confidence: {category.Score:p})");
        }
    }

    public void DisplayVisionObjects(ImageAnalysis imageAnalysis)
    {
        if (!imageAnalysis.Objects.Any()) return;

        Console.WriteLine("\r\nObjects:");
        foreach (DetectedObject? obj in imageAnalysis.Objects)
        {
            if (obj == null) continue;

            Console.WriteLine($"{obj.ObjectProperty} (Confidence: {obj.Confidence:p}) at ({obj.Rectangle.X},{obj.Rectangle.Y}):({obj.Rectangle.X + obj.Rectangle.W},{obj.Rectangle.Y + obj.Rectangle.H})");
            ObjectHierarchy? parent = obj.Parent;
            while (parent != null)
            {
                Console.WriteLine($"Parent: {parent.ObjectProperty} (Confidence: {parent.Confidence:p})");
                parent = parent.Parent;
            }
        }
    }

    public void DisplayVisionTags(ImageAnalysis imageAnalysis)
    {
        if (!imageAnalysis.Tags.Any()) return;
        
        Console.WriteLine("\r\nTags:");
        foreach (ImageTag? tag in imageAnalysis.Tags)
        {
            if (tag == null) continue;

            Console.WriteLine($"{tag.Name} (Confidence: {tag.Confidence:p})");
        }
    }

}