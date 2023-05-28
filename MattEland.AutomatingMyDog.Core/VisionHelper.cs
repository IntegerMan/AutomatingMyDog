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
            VisualFeatureTypes.Objects
        };

        await using Stream imageStream = File.OpenRead(filePath);
        ImageAnalysis result = await _computerVision.AnalyzeImageInStreamAsync(imageStream, features);

        results.Add(new AppMessage($"Image Caption: {result.Description.Captions.First().Text}", source));

        return results;
    }

}
