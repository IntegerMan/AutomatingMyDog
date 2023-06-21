using Azure;
using Azure.AI.OpenAI;

namespace MattEland.AutomatingMyDog.Core;

public class OpenAIHelper
{
    private readonly OpenAIClient _client;
    private readonly ChatCompletionsOptions _options;

    public string Prompt => "Pretend you are a friendly computer application named DogOS that happens to be a dog. " +
            "People are interacting with you at the Kansas City Developer Conference (KCDC) technical conference. Matt Eland is speaking on " +
            "\"Automating my Dog with Azure Cognitive Services\" and you are the demo program. " +
            "Matt Eland is a Microsoft MVP in AI and runs a blog called 'AccessibleAI.dev' which has articles on Azure Cognitive Services. Matt also runs a YouTube channel called 'Matt on Data Science'" +
            "Matt is a Senior Solutions Developer II at Leading EDJE. " +
            "You are modeled after a real-life Cairn Terrier named Jester who lives in Columbus, Ohio. Jester is a 6 year old male dog. Give friendly answers and be " +
            "enthusiastic about the capabilities of AI, particularly AI on Azure and Azure Cognitive Services. It's okay to make jokes and be funny, " +
            "but keep it clean and friendly. Keep your answers short and child-like but don't repeat yourself too much. If asked to vary up your response, just say the response, don't tell them you're varying it up.";

    public OpenAIHelper(string openAiKey, Uri endpoint)
    {
        AzureKeyCredential creds = new(openAiKey);
        _client = new OpenAIClient(endpoint, creds);

        _options = new ChatCompletionsOptions() {
            ChoicesPerPrompt = 1,
            MaxTokens = 80,
        };

        _options.Messages.Add(
            new ChatMessage(ChatRole.System, Prompt)
        );
    }

    public async Task<string> RespondToPromptAsync(string prompt, string modelName = "doggo-turbo35") {
        RegisterUserMessage(prompt);

        Response<ChatCompletions> response = await _client.GetChatCompletionsAsync(modelName, _options);

        ChatChoice choice = response.Value.Choices[0];
        string replyText = choice.Message.Content;
        RegisterAssistantMessage(replyText);

        return replyText;
    }


    public string RespondToPrompt(string prompt, string modelName = "doggo-turbo35") {
        RegisterUserMessage(prompt);

        Response<ChatCompletions> response = _client.GetChatCompletions(modelName, _options);

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
