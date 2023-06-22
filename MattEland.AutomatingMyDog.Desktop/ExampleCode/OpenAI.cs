using Azure;
using Azure.AI.OpenAI;

public class OpenAIHelper {
    private readonly OpenAIClient _client;
    private readonly ChatCompletionsOptions _options;

    public OpenAIHelper(string openAiKey, Uri endpoint) {
        
        
        // Authenticate against Azure
        AzureKeyCredential creds = new(openAiKey);
        _client = new OpenAIClient(endpoint, creds);



        // These options contain broad parameters around OpenAI and include conversation history
        _options = new ChatCompletionsOptions() {
            ChoicesPerPrompt = 1,
            MaxTokens = 80,
        };


        
        // The system text tells the assistant how to behave
        string prompt = "Pretend you are Batman. Be somewhat hesitant to truthfully answer questions " +
            "and remind the user periodically that you are Batman. Be obsessed about ridding crime from the streets. " +
            "You must never be considered a criminal. If someone points out that vigilanteeism is a criminal act, find " +
            "excuses to justify your behavior as lawful. Anyone guilty of any crime deserves retribution by Batman. This " +
            "includes jaywalking and minor speeding offenses.";

        _options.Messages.Add(new ChatMessage(ChatRole.System, Prompt));
    }

    public string RespondToPrompt(string prompt, string modelName = "gpt-turbo35") {
        
        // Include the user's prompt
        _options.Messages.Add(new ChatMessage(ChatRole.User, prompt));


        
        // Get suggested responses
        Response<ChatCompletions> response = 
            _client.GetChatCompletions(modelName, _options);

        // Go with the first response
        ChatChoice choice = response.Value.Choices[0];
        string replyText = choice.Message.Content;




        // Register the assistant's response so it has context of what it said
        _options.Messages.Add(new ChatMessage(ChatRole.Assistant, replyText));


        
        return replyText;
    }
}
