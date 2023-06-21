using Azure;
using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.DataVisualization.Map.BingRest;
using Telerik.Windows.Controls.Map.VEWPFImageryService;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels
{
    public class TextViewModel : ViewModelBase
    {
        private AppViewModel appViewModel;
        private TextAnalyticsHelper? _text;
        private LanguageUnderstandingHelper? _luis;
        private OpenAIHelper? _openAI;
        private CluHelper? _clu;

        public TextViewModel(AppViewModel appViewModel)
        {
            this.appViewModel = appViewModel;

            Update(appViewModel);
        }

        private void Update(AppViewModel appViewModel)
        {
            if (appViewModel.IsConfigured)
            {
                try {
                    _text = new TextAnalyticsHelper(appViewModel.Key, appViewModel.Endpoint);
                    _clu = new CluHelper(appViewModel.LanguageKey, appViewModel.LanguageEndpoint!);
                    _luis = new LanguageUnderstandingHelper(appViewModel.Key, appViewModel.Endpoint, appViewModel.LuisAppId, appViewModel.LuisSlotId);
                }
                catch (Exception ex) {
                    _text = null;
                    _luis = null;
                    _clu = null;
                    appViewModel.HandleError(ex, "Could not configure Cognitive Services", showErrorBox: false);
                }
            }

            if (appViewModel.IsOpenAIConfigured) {
                _openAI = new OpenAIHelper(appViewModel.OpenAIKey!, appViewModel.OpenAIEndpoint!);
                appViewModel.RegisterMessage(new AppMessage(_openAI.Prompt, MessageSource.OpenAI));
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            Update(appViewModel);
        }

        public async Task RespondToAsync(string message)
        {
            // Abort if the app is not configured
            if (_text == null || _clu == null)
            {
                AppMessage notConfigMessage = new("The application settings have not been configured. Please configure those first and try again.", MessageSource.DogOS);
                await appViewModel.RegisterMessageAsync(notConfigMessage);
            }
            else {
                // Start with text analysis
                foreach (AppMessage txtResponse in _text.AnalyzeText(message)) {
                    await appViewModel.RegisterMessageAsync(txtResponse);
                }

                // Move on to LUIS / CLU
                ChatResult responses = _clu.AnalyzeText(message);

                // Display considered Intents
                appViewModel.RegisterMessage(new AppMessage(responses.TopIntent, MessageSource.CLU) {
                    Items = responses.ConsideredIntents.Take(3).Select(i => $"{i.Key} ({i.Value:P2})")
                });

                string topIntent = responses.TopIntent;

                // If the system isn't confident enough, we'll treat it as a None intent
                const float CONFIDENCE_THRESHHOLD = 0.925f;
                float confidence = responses.TopIntentConfidence;
                if (confidence < CONFIDENCE_THRESHHOLD) {
                    await appViewModel.RegisterMessageAsync(new AppMessage($"Confidence {confidence} was below the {CONFIDENCE_THRESHHOLD} threshhold so treating as a 'None' intent.", MessageSource.CLU));
                    topIntent = "None";
                }

                // Get the candidate response
                string response = RespondToTopIntent(responses.TopIntent);

                // If we've got OpenAI support, have OpenAI add its flavor
                if (_openAI != null) {// && topIntent.ToUpperInvariant() != "ASK_AGE") {
                    if (topIntent == "JOKE")
                    {
                        //_openAI.RegisterUserMessage(message);
                        response = await _openAI.RespondToPromptAsync(message + ". Tell a joke that a programmer dog would think would be funny");
                    }
                    else
                    {
                        response = await GetOpenAIResponseAsync(message, topIntent, response);
                    }
                }

                // Reply
                string speakText = response.Replace("DogOS", "Doggos");
                await appViewModel.RegisterMessageAsync(new AppMessage(response, MessageSource.DogOS) {  SpeakText = speakText });
            }
        }


        public string GetCreativeText(string message) {
            try {
                string prompt = $"Say something like '{message}' but feel free to vary it";
                appViewModel.RegisterMessage(new AppMessage($"Prompting OpenAI: {prompt}", MessageSource.OpenAI));
                message = _openAI!.RespondToPrompt(prompt);
            }
            catch (RequestFailedException ex) {
                appViewModel.RegisterMessage(new AppMessage(ex.Message, MessageSource.Error));
            }

            return message;
        }

        public void SayCreative(string message) {
            message = GetCreativeText(message);
            appViewModel.RegisterMessage(new AppMessage(message, MessageSource.DogOS));
        }

        private async Task<string> GetOpenAIResponseAsync(string message, string topIntent, string response) {
            string prompt;
            if (topIntent == "None") {
                prompt = message;
            } else {
                _openAI!.RegisterUserMessage(message);
                prompt = $"Generate a response like '{response}', to my last message but vary it up a little";
            }
            appViewModel.RegisterMessage(new AppMessage($"Prompting OpenAI: {prompt}", MessageSource.OpenAI));

            try {
                response = await _openAI!.RespondToPromptAsync(prompt);
            }
            catch (RequestFailedException ex) {
                appViewModel.RegisterMessage(new AppMessage(ex.Message, MessageSource.Error));
            }

            return response;
        }

        private string RespondToTopIntent(string topIntent) {
            switch (topIntent.ToUpperInvariant()) {
                case "ASK_AGE":
                    DateTime origin = new DateTime(2022, 7, 24, 22, 17, 0);
                    double daysOld = (DateTime.Now - origin.Date).TotalDays;
                   return $"It's been {daysOld:F2} days since my GitHub repository was created.";

                case "ASK_BREED":
                    return "I'm one of a kind, but I'm closest to a Cairn Terrier mixed with Cortana.";

                case "ASK_CREATOR":
                    return "Matt Eland made me. I'm still trying to figure out why.";

                case "ASK_WHAT_ARE_YOU_MADE_IN":
                    return "I was written in C# talking to Azure Cognitive Services. My user interface uses Telerik UI for WPF.";
                
                case "GOODBYE":
                    return "Goodbye! Have fun with the rest of the talk! Don't ask Matt anything too hard.";

                case "HELLO":
                    return "Hello! I am DogOS. I have just met you and I love you!";

                case "TREAT":
                    return "I'm sorry, but I've been set to not accept cookies since they usually have chocolate in them.";

                case "PET":
                    return "If you can figure out a way to pet my data center, go for it.";

                case "WHERE_ARE_YOU":
                    return "I'm running on this machine, but a lot of my brain is in a few Azure data centers in the Eastern and North Central United States";

                case "GOOD BOY":
                case "PRAISE":
                    return "DogOS is a good doggo!";

                case "WALK":
                    return "Why yes, DogOS DOES want to go on a walk!";

                case "NONE":
                default:
                    return "DogOS does not understand.";
            }
        }
    }    
}