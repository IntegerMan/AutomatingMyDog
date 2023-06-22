using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

private ComputerVisionClient CreateClient() {


    return computerVision;
}

public async Task GenerateThumbnail(string sourceImagePath, string outputFile) {



    // Read in the source image
    using FileStream inputStream = new(sourceImagePath, FileMode.Open, FileAccess.Read);

    
    
    // These should come from a config file
    string key = "123abc45def67g89h0i12345jk6lmno7";
    string endpoint = "https://yourendpoint.cognitiveservices.azure.com/";

    // Authenticate with a Azure Cognitive Services or a Computer Vision resource
    ComputerVisionClient client = new(key);
    computerVision.Endpoint = endpoint;


    
    
    // Have Computer Vision on Azure find the region of interest and crop it
    const int Width = 200;
    const int Height = 200;
    using Stream croppedStream = await client.GenerateThumbnailInStreamAsync(
        Width, 
        Height, 
        inputStream);



    
    // Take the output image and write it to the output thumbnail file
    using FileStream outputStream = new(outputFile, FileMode.Create, FileAccess.Write);
    
    croppedStream.CopyTo(outputStream);


    
}
