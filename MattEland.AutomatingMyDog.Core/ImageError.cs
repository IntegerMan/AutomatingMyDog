namespace MattEland.AutomatingMyDog.Core;

public record ImageErrorResponse(ImageError Error);

public record ImageError(string Code, string Message);
