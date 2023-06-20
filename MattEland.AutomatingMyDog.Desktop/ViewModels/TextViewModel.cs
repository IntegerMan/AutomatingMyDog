using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
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
                _openAI = new OpenAIHelper(appViewModel.OpenAIKey, appViewModel.OpenAIEndpoint!);
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
                foreach (AppMessage response in _text.AnalyzeText(message)) {
                    await appViewModel.RegisterMessageAsync(response);
                }

                // Move on to LUIS / CLU
                ChatResult responses = _clu.AnalyzeText(message);

                // Display considered Intents
                appViewModel.RegisterMessage(new AppMessage(responses.TopIntent, MessageSource.CLU) {
                    Items = responses.ConsideredIntents.Take(3).Select(i => $"{i.Key} ({i.Value:P2})")
                });

                string topIntent = responses.TopIntent;

                // If the system isn't confident enough, we'll treat it as a None intent
                const float CONFIDENCE_THRESHHOLD = 0.8f;
                float confidence = responses.TopIntentConfidence;
                if (confidence < CONFIDENCE_THRESHHOLD) {
                    appViewModel.RegisterMessage(new AppMessage($"Confidence {confidence} was below the {CONFIDENCE_THRESHHOLD} threshhold so treating as a 'None' intent.", MessageSource.CLU));
                    topIntent = "None";
                }

                // Actually respond
                RespondToTopIntent(responses.TopIntent);
            }
        }

        private void RespondToTopIntent(string topIntent) {


            switch (topIntent.ToUpperInvariant()) {
                case "ASK_AGE":
                    DateTime origin = new DateTime(2022, 7, 24, 22, 17, 0);
                    double daysOld = (DateTime.Now - origin.Date).TotalDays;
                    appViewModel.RegisterMessage(new AppMessage($"It's been {daysOld} days since my GitHub repository was created.", MessageSource.DogOS));
                    break;

                case "ASK_BREED":
                    appViewModel.RegisterMessage(new AppMessage("I'm one of a kind, but I'm closest to a Cairn Terrier mixed with Cortana.", MessageSource.DogOS));
                    break;

                case "ASK_CREATOR":
                    appViewModel.RegisterMessage(new AppMessage("Matt Eland made me. I'm still trying to figure out why.", MessageSource.DogOS));
                    break;

                case "ASK_WHAT_ARE_YOU_MADE_IN":
                    appViewModel.RegisterMessage(new AppMessage("I was written in C# talking to Azure Cognitive Services. My user interface uses Telerik UI for WPF.", MessageSource.DogOS));
                    break;

                case "GOODBYE":
                    appViewModel.RegisterMessage(new AppMessage("Goodbye! Have fun with the rest of the talk! Don't ask Matt anything too hard.", MessageSource.DogOS));
                    break;

                case "HELLO":
                    appViewModel.RegisterMessage(new AppMessage("Hello! I am DogOS. I have just met you and I love you!", MessageSource.DogOS) {  SpeakText = "Hello! I am Doggos. I have just met you and I love you!" });
                    break;

                case "WHERE_ARE_YOU":
                    appViewModel.RegisterMessage(new AppMessage("I'm running on this machine, but a lot of my brain is in a few Azure data centers in the Eastern and North Central United States", MessageSource.DogOS));
                    break;

                case "GOOD BOY":
                case "PRAISE":
                    appViewModel.RegisterMessage(new AppMessage("DogOS is a good doggo!", MessageSource.DogOS) { SpeakText = "Doggos is a good doggo." });
                    break;

                case "WALK":
                    appViewModel.RegisterMessage(new AppMessage("Why yes, DogOS DOES want to go on a walk!", MessageSource.DogOS));
                    break;

                case "NONE":
                default:
                    appViewModel.RegisterMessage(new AppMessage("DogOS does not understand.", MessageSource.DogOS) { SpeakText = "Doggos does not understand." });
                    break;
            }
        }
    }    
}