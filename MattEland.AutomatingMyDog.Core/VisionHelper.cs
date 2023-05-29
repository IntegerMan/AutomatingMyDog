using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

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
        const MessageSource source = MessageSource.ComputerVision;

        // We need to tell it what types of results we care about
        List<VisualFeatureTypes?> features = new()
        {
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Description,
            VisualFeatureTypes.ImageType,
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
        results.Add(new AppMessage($"Adult Content: {adult.IsAdultContent} Confidence in Adult / Gore / Racy: {adult.AdultScore:P} / {adult.GoreScore:P} / {adult.RacyScore:P}", source));

        // Color Message
        results.Add(new AppMessage($"Accent Color: {result.Color.AccentColor} Dominant Color: {result.Color.DominantColorForeground} foreground / {result.Color.DominantColorBackground}", source));

        // Image Type
        results.Add(new AppMessage($"Clip Art Type: {result.ImageType.ClipArtType} Line Drawing Type: {result.ImageType.LineDrawingType}", source));

        // Tags
        results.Add(new AppMessage($"Tags: {string.Join(", ", result.Tags.Select(t => t.Name))}", source));

        // Categories
        results.Add(new AppMessage($"Categories: {string.Join(", ", result.Categories.Select(c => c.Name))}", source));

        // Brands
        results.Add(new AppMessage($"Brands: {string.Join(", ", result.Brands.Select(b => b.Name))}", source));

        // Objects
        results.Add(new AppMessage($"Objects: {string.Join(", ", result.Objects.Select(o => o.ObjectProperty))}", source));

        return results;
    }

}
