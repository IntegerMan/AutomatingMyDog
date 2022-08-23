namespace MattEland.AutomatingMyDog;

public record AutomatingMyDogConfig(string Key, 
    string Endpoint, 
    string Region,
    string AppId,
    string SlotId)
{
}