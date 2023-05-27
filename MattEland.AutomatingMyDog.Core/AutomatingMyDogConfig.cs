namespace MattEland.AutomatingMyDog.Core;

public record AutomatingMyDogConfig(string Key,
    string Endpoint,
    string Region,
    string AppId,
    string SlotId)
{
}