using MattEland.AutomatingMyDog.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels
{
    public class SpeechViewModel : ViewModelBase
    {
        private readonly AppViewModel _vm;
        private SpeechHelper? _speech;

        public IEnumerable<string> Voices { get; } = new string[]
        {
            "en-US-GuyNeural",
            "en-US-JennyMultilingualNeural",
            "en-US-AnaNeural",
            "en-CA-LiamNeural",
            "en-CA-ClaraNeural",
            "en-GB-MaisieNeural",
            "en-GB-NoahNeural",
            "en-AU-DuncanNeural",
            "en-AU-ElsieNeural",
            "en-SG-LunaNeural",
            "en-KE-AsiliaNeural",
            "en-IE-EmilyNeural"
        };

        public SpeechViewModel(AppViewModel appViewModel)
        {
            _vm = appViewModel;

            UpdateTextToSpeech(appViewModel);
        }

        private void UpdateTextToSpeech(AppViewModel appViewModel)
        {
            if (appViewModel.IsConfigured || _speech == null)
            {
                _speech = new SpeechHelper(appViewModel.Key, appViewModel.Region, appViewModel.Voice);
            } 
            else
            {
                _speech.VoiceName = appViewModel.Voice;
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            UpdateTextToSpeech(appViewModel);
        }

        public async Task SayAsync(string message, string? speechText=null)
        {
            speechText ??= message.Replace("DogOS", "Doggos");

            // If the user provided speech text to customize the pronunciation, use that. Otherwise, use the message.
            if (_speech != null && _vm.UseSpeech) {
                bool result = await _speech.SayMessageAsync(speechText ?? message);
                if (!result) {
                    await _vm.RegisterMessageAsync(new AppMessage("Could not generate speech. You may be offline or your Azure AI Services settings are not correctly configured", MessageSource.Error));
                }
            }
        }

        public string ListenForText()
        {
            try
            {
                if (_speech == null) { throw new SpeechException("Speech is not configured. Configure application settings first"); }
                return _speech.ListenToSpokenText();
            }
            catch (SpeechException ex)
            {
                _vm.HandleError(ex, "Could not detect audio");

                return string.Empty;
            }
        }
    }
}