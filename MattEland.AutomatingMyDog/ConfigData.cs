namespace MattEland.AutomatingMyDog;

public record ConfigData
{
    public ConfigData(string key, string endpoint)
    {
        Key = key;
        Endpoint = endpoint;
    }

    public string Key { get; init; }
    public string Endpoint { get; init; }
}