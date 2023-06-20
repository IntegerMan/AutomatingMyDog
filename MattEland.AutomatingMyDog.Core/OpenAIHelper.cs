using Azure;
using Azure.AI.OpenAI;

namespace MattEland.AutomatingMyDog.Core;

public class OpenAIHelper
{
    private readonly OpenAIClient _client;
    private readonly ChatCompletionsOptions _options;

    public OpenAIHelper(string openAiKey, Uri endpoint)
    {
        AzureKeyCredential creds = new(openAiKey);
        _client = new OpenAIClient(endpoint, creds);

        _options = new ChatCompletionsOptions();
        _options.Messages.Add(
            new ChatMessage(ChatRole.System, "Pretend you are a friendly computer application named DogOS that happens to be a dog. " +
            "People are interacting with you at a technical conference. Give friendly answers and be enthusiastic about the capabilities of AI, " +
            "particularly AI on Azure and Azure Cognitive Services. It's okay to make jokes and be funny, but keep it clean and friendly. " +
            "Keep your answers short and child-like")
        );
    }

    public async Task<string> RespondToPromptAsync(string prompt, string modelName = "text-davinci-003") {
        RegisterUserMessage(prompt);

        Response<ChatCompletions> response = await _client.GetChatCompletionsAsync(modelName, _options);

        ChatChoice choice = response.Value.Choices[0];
        string replyText = choice.Message.Content;
        RegisterAssistantMessage(replyText);

        return replyText;
    }

    public void RegisterUserMessage(string prompt) {
        _options.Messages.Add(new ChatMessage(ChatRole.User, prompt));
    }

    public void RegisterAssistantMessage(string response) {
        _options.Messages.Add(new ChatMessage(ChatRole.Assistant, response));
    }

}
