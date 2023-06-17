using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System.Drawing;

namespace MattEland.AutomatingMyDog.Core;

#pragma warning disable CA1416 // Validate platform compatibility

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
        string outputFile = Path.GetTempFileName();
        const int Width = 200;
        const int Height = 200;

        // Generate a smart thumbnail and save it to the thumbnail file
        try 
        {
            using (FileStream inputStream = new(filePath, FileMode.Open, FileAccess.Read)) {
                await using (Stream croppedStream = await _computerVision.GenerateThumbnailInStreamAsync(Width, Height, inputStream, smartCropping: true)) {
                    using (FileStream outputStream = new(outputFile, FileMode.Create, FileAccess.Write)) {
                        croppedStream.CopyTo(outputStream);
                    }
                }
            }
        }
        catch (HttpOperationException ex) {
            return HandleImageError(ex);
        }

        List<AppMessage> results = new();
        List<string> detectedItems = new();

        const MessageSource source = MessageSource.ComputerVision;
        ImageAnalysis result = await AnalyzeImage(filePath);

        // Describe Image with Caption
        results.Add(new AppMessage("Captioning", source)
        {
            ImagePath = outputFile,
            UseLandscapeLayout = true,
            Items = result.Description.Captions.Select(c => $"{c.Text} ({c.Confidence:P})"),
        });

        // Adult / Racy Message
        AdultInfo adult = result.Adult;
        results.Add(new AppMessage("Content Moderation", source)
        {
            Items = new List<string>
            {
                $"Adult Content: {adult.AdultScore:P}",
                $"Racy Content: {adult.RacyScore:P}",
                $"Gory Content: {adult.GoreScore:P}",
            },
        });

        // Color Message
        Color accentColor = Color.Green;
        if (result.Color.AccentColor != null || result.Color.DominantColors.Any())
        {
            HashSet<string> colors = GetColorsFromResult(result.Color);

            // Convert the hex code to a Color object
            accentColor = ColorTranslator.FromHtml($"#{result.Color.AccentColor}");

            // Create a bitmap image to include in the card and represent the accent color
            string accentImage = Path.GetTempFileName();
            CreateColorImage(accentColor, accentImage);

            results.Add(new AppMessage("Colors", source)
            {
                Items = colors,
                ImagePath = accentImage,
                UseLandscapeLayout = true,
            });
        }

        // Tags
        if (result.Tags.Any())
        {
            detectedItems.AddRange(result.Tags.Select(t => t.Name));
            results.Add(new AppMessage("Tags", source) {
                Items = result.Tags.Select(t => $"{t.Name} ({t.Confidence:P}) {t.Hint}"),
            });
        }

        // Categories
        if (result.Categories.Any())
        {
            detectedItems.AddRange(result.Categories.Select(t => t.Name));

            List<string> cats = new();
            foreach (Category category in result.Categories)
            {
                cats.Add($"{category.Name} ({category.Score:P})");
                foreach (LandmarksModel landmark in category.Detail?.Landmarks ?? Enumerable.Empty<LandmarksModel>())
                {
                    cats.Add($"Landmark: {landmark.Name} ({landmark.Confidence:P})");
                }
                foreach (CelebritiesModel celebrity in category.Detail?.Celebrities ?? Enumerable.Empty<CelebritiesModel>())
                {
                    cats.Add($"Celebrity: {celebrity.Name} ({celebrity.Confidence:P})");
                }
            }   
            results.Add(new AppMessage("Categories", source)
            {
                Items = cats,
            });
        }

        // Brands
        if (result.Brands.Any())
        {
            results.Add(new AppMessage("Brands", source)
            {
                Items = result.Brands.Select(b => b.Name)
            });
        }

        // Objects
        if (result.Objects.Any())
        {
            detectedItems.AddRange(result.Objects.Select(t => t.ObjectProperty));

            // Optionally add bounding boxes to an image
            string? output_file = BuildBoundingBoxFile(filePath, result, accentColor);

            results.Add(new AppMessage("Objects", source)
            {
                ImagePath = output_file,
                Items = result.Objects.Select(o => o.ObjectProperty),
            });
        }

        // Determine if we should "bark" at something
        string message = GenerateBarkMessage(detectedItems);
        results.Add(new AppMessage(message, MessageSource.DogOS));

        return results;
    }

    private static IEnumerable<AppMessage> HandleImageError(HttpOperationException ex) {
        // Deserialize ex.Response to ImageError class if it exists
        ImageErrorResponse? errorResponse = null;
        if (!string.IsNullOrEmpty(ex.Response.Content)) {
            errorResponse = JsonConvert.DeserializeObject<ImageErrorResponse>(ex.Response.Content);
        }

        string errorMessage = "Could not analyze image: ";
        if (!string.IsNullOrWhiteSpace(errorResponse?.Error?.Message)) {
            errorMessage += errorResponse.Error.Message;
        } else {
            errorMessage += ex.Message;
        }

        // Handle cases where we got a non-success response
        return new List<AppMessage>() {
                new AppMessage(errorMessage, MessageSource.Error) {
                    SpeakText = errorMessage,
                }
            };
    }

    private static HashSet<string> GetColorsFromResult(ColorInfo colorInfo)
    {
        HashSet<string> colors = new()
        {
            $"#{colorInfo.AccentColor} (Accent)",
            $"{colorInfo.DominantColorForeground} (Foreground)",
            $"{colorInfo.DominantColorBackground} (Background)",
        };

        // Add any additional colors
        foreach (string color in colorInfo.DominantColors.Where(c => c != colorInfo.DominantColorForeground &&
                                                                     c != colorInfo.DominantColorBackground))
        {
            colors.Add(color);
        }

        return colors;
    }

    private static void CreateColorImage(Color color, string accentImage)
    {
        using (Bitmap bmp = new(8, 8))
        {
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(color);
            bmp.Save(accentImage);
        }
    }

    private async Task<ImageAnalysis> AnalyzeImage(string filePath)
    {
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

        await using (Stream imageStream = File.OpenRead(filePath))
        return await _computerVision.AnalyzeImageInStreamAsync(imageStream, features);
    }

    private static string? BuildBoundingBoxFile(string filePath, ImageAnalysis result, Color boundingBoxColor)
    {
        // TODO: Only do this on Windows
        Image image = Image.FromFile(filePath);
        Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(boundingBoxColor, 5);
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
        string output_file = Path.GetTempFileName();
        image.Save(output_file);

        return output_file;
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
            IEnumerable<string> itemsToMention = detectedItems.Distinct()
                                                              .Take(5)
                                                              .Select(i => i.Replace("_", ""));
            message = $"Nothing to bark at, but here's some things I saw: {string.Join(", ", itemsToMention)}";
        }

        return message;
    }
}
#pragma warning restore CA1416 // Validate platform compatibility
