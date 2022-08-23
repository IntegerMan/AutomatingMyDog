namespace MattEland.AutomatingMyDog;

public static class StringHelper
{
    public static string AddArticleIfNotPresent(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        string lower = input.ToLower();

        if (lower.StartsWith("a ") || lower.StartsWith("an ") || lower.StartsWith("the ") || lower.StartsWith("some "))
        {
            return input;
        }

        string article = IsVowel(input[0]) ? "an" : "a";
        return $"{article} {input}";
    }

    public static bool IsVowel(char c) => char.ToLower(c) is 'a' or 'e' or 'i' or 'o' or 'u';
}