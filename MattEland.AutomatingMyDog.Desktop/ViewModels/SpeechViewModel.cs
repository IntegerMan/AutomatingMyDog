using MattEland.AutomatingMyDog.Core;
using System;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels
{
    public class SpeechViewModel : ViewModelBase
    {
        private AppViewModel appViewModel;
        private TextToSpeechHelper? _speech;

        public SpeechViewModel(AppViewModel appViewModel)
        {
            this.appViewModel = appViewModel;

            UpdateTextToSpeech(appViewModel);
        }

        private void UpdateTextToSpeech(AppViewModel appViewModel)
        {
            if (appViewModel.IsConfigured)
            {
                _speech = new TextToSpeechHelper(appViewModel.Key, appViewModel.Region);
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            UpdateTextToSpeech(appViewModel);
        }

        public void Say(string message)
        {
            _speech?.SayMessage(message);
        }
    }
}