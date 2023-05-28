using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using System;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels
{
    public class SpeechViewModel : ViewModelBase
    {
        private AppViewModel _vm;
        private SpeechHelper? _speech;

        public SpeechViewModel(AppViewModel appViewModel)
        {
            _vm = appViewModel;

            UpdateTextToSpeech(appViewModel);
        }

        private void UpdateTextToSpeech(AppViewModel appViewModel)
        {
            if (appViewModel.IsConfigured)
            {
                _speech = new SpeechHelper(appViewModel.Key, appViewModel.Region);
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            UpdateTextToSpeech(appViewModel);
        }

        public void Say(string message, string? speechText=null)
        {
            // If the user provided speech text to customize the pronunciation, use that. Otherwise, use the message.
            _speech?.SayMessage(speechText ?? message);
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