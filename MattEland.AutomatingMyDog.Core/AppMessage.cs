namespace MattEland.AutomatingMyDog.Core;

public record AppMessage
{
    public AppMessage(string message, MessageSource source)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Source = source;
    }

    public string Message { get; set; }
    public MessageSource Source { get;  }
    public string? SpeakText { get; init; }
    public string? ImagePath { get; init; }
    public IEnumerable<string>? Items { get; init; }
    public bool UseLandscapeLayout { get; internal set; }
}
