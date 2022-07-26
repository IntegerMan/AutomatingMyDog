namespace MattEland.AutomatingMyDog;

public record ConfigData(string Key, 
    string Endpoint, 
    string Region,
    string AppId,
    string SlotId)
{
}