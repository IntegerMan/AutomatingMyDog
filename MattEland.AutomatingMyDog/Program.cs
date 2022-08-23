namespace MattEland.AutomatingMyDog;

public class Program
{
    public static void Main()
    {
        // Read credentials from credentials.json
        ConfigurationManager configManager = new();
        AutomatingMyDogConfig configData = configManager.LoadConfigData();

        // Show the main menu
        AutomatingMyDogMenu menu = new(configData);
        menu.ShowMainMenu();
    }
}