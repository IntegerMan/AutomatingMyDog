namespace MattEland.AutomatingMyDog;

public static class ImageSelectionHelper
{
    public static string? SelectImage(string[] files)
    {
        // List all Files
        Console.WriteLine("Available Images:");
        Console.WriteLine();

        // Determine the largest width we can have for an image index for alignment purposes
        int maxWidth = (files.Length + 1).ToString().Length;

        int index = 0;
        foreach (string file in files)
        {
            index++;
            string indexLabel = index.ToString().PadLeft(maxWidth);

            // Use a FileInfo so we can just show the name of the file, not the full path
            FileInfo info = new(file);

            Console.WriteLine($"{indexLabel}) {info.Name}");
        }

        // Get a response from the user
        Console.WriteLine();
        Console.WriteLine("Which file would you like me look at?");
        Console.WriteLine();

        string fileText = Console.ReadLine()!;

        string? imagePath = null;

        // Translate the user's input to a file name, or use null if the input is invalid
        if (int.TryParse(fileText, out int fileIndex))
        {
            if (fileIndex > 0 && fileIndex <= files.Length)
            {
                imagePath = files[fileIndex - 1];
            }
            else
            {
                Console.WriteLine("That is not a valid choice");
            }
        }
        else
        {
            Console.WriteLine("That is not a valid number. Please enter the index of the image to look at.");
        }

        return imagePath;
    }

}