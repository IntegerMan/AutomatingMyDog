using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace MattEland.AutomatingMyDog.Core;

public class VisionHelper
{
    private readonly ComputerVisionClient _computerVision;

    public VisionHelper(string subscriptionKey, string endpoint)
    {
        if (string.IsNullOrWhiteSpace(subscriptionKey))
        {
            throw new ArgumentException($"'{nameof(subscriptionKey)}' cannot be null or whitespace.", nameof(subscriptionKey));
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or whitespace.", nameof(endpoint));
        }

        ApiKeyServiceClientCredentials visionCredentials = new(subscriptionKey);
        _computerVision = new ComputerVisionClient(visionCredentials);
        _computerVision.Endpoint = endpoint;
    }

    public async Task<IEnumerable<AppMessage>> AnalyzeImageAsync(string filePath)
    {
        List<AppMessage> results = new();
        List<string> detectedItems = new();

        const MessageSource source = MessageSource.ComputerVision;

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

        await using Stream imageStream = File.OpenRead(filePath);
        ImageAnalysis result = await _computerVision.AnalyzeImageInStreamAsync(imageStream, features);

        // Describe Image with Caption
        results.Add(new AppMessage($"Image Caption: {result.Description.Captions.First().Text}", source));

        // Adult / Racy Message
        AdultInfo adult = result.Adult;
        results.Add(new AppMessage($"Confidence in Adult / Gore / Racy content in the image: {adult.AdultScore:P} / {adult.GoreScore:P} / {adult.RacyScore:P}", source));

        // Color Message
        if (result.Color.DominantColors.Any())
        {
            results.Add(new AppMessage($"Dominant Colors: {string.Join(", ", result.Color.DominantColors)}. Accent Color: {result.Color.AccentColor}", source));
        }

        // Tags
        if (result.Tags.Any())
        {
            detectedItems.AddRange(result.Tags.Select(t => t.Name));
            results.Add(new AppMessage($"Tags: {string.Join(", ", result.Tags.Select(t => t.Name))}", source));
        }

        // Categories
        if (result.Categories.Any())
        {
            detectedItems.AddRange(result.Categories.Select(t => t.Name));
            results.Add(new AppMessage($"Categories: {string.Join(", ", result.Categories.Select(c => c.Name))}", source));
        }

        // Brands
        if (result.Brands.Any())
        {
            results.Add(new AppMessage($"Brands: {string.Join(", ", result.Brands.Select(b => b.Name))}", source));
        }

        // Objects
        if (result.Objects.Any())
        {
            detectedItems.AddRange(result.Objects.Select(t => t.ObjectProperty));

            // Add bounding boxes to an image
            // TODO: Only do this on Windows
#pragma warning disable CA1416 // Validate platform compatibility
            var image = System.Drawing.Image.FromFile(filePath);
            Graphics graphics = Graphics.FromImage(image);
            Pen pen = new(Color.Green, 5);
            Font font = new("Arial", 16, FontStyle.Bold);
            SolidBrush brush = new(Color.Black);

            foreach (DetectedObject detectedObject in result.Objects)
            {
                // Draw object bounding box
                var r = detectedObject.Rectangle;
                Rectangle rect = new(r.X, r.Y, r.W, r.H);
                graphics.DrawRectangle(pen, rect);
                graphics.DrawString(detectedObject.ObjectProperty, font, brush, r.X, r.Y);
            }

            // Save annotated image
            string output_file = Path.Combine(Environment.CurrentDirectory, "objects.jpg");
            image.Save(output_file);
#pragma warning restore CA1416 // Validate platform compatibility

            results.Add(new AppMessage($"Objects: {string.Join(", ", result.Objects.Select(o => o.ObjectProperty))}", source)
            {
                ImagePath = output_file,                
            });
        }

        // Determine if we should "bark" at something
        string message = GenerateBarkMessage(detectedItems);
        results.Add(new AppMessage(message, MessageSource.DogOS));

        return results;
    }

    private static string GenerateBarkMessage(List<string> detectedItems)
    {
        string? barkTarget = detectedItems.FirstOrDefault(t => t.IsSomethingToBarkAt());
        string message;
        if (barkTarget != null)
        {
            // Add the word "a" in front of it if needed, but only if it doesn't start with an article already
            barkTarget = StringHelper.AddArticleIfNotPresent(barkTarget);

            message = $"I saw {barkTarget}; Bark, bark, bark!";
        }
        else
        {
            IEnumerable<string> itemsToMention = detectedItems.Distinct().Take(5);
            message = $"Nothing to bark at, but here's some things I saw: {string.Join(", ", itemsToMention)}";
        }

        return message;
    }
}
