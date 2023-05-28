using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using System;
using System.Collections.Generic;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels
{
    public class TextViewModel : ViewModelBase
    {
        private AppViewModel appViewModel;
        private TextAnalyticsHelper? _text;

        public TextViewModel(AppViewModel appViewModel)
        {
            this.appViewModel = appViewModel;

            Update(appViewModel);
        }

        private void Update(AppViewModel appViewModel)
        {
            if (appViewModel.IsConfigured)
            {
                _text = new TextAnalyticsHelper(appViewModel.Key, appViewModel.Endpoint);
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            Update(appViewModel);
        }

        public IEnumerable<ChatMessageViewModel> RespondTo(string message)
        {
            // Abort if the app is not configured
            if (_text == null)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Not Configured",
                    Content = "The application settings have not been configured. Please configure those first and try again.",
                });

                yield break;
            }
            else
            {
                foreach (string response in _text.AnalyzeText(message))
                {
                    appViewModel.RegisterMessage(new ChatMessageViewModel(response, Chat.TextAnalysisAuthor));
                }
            }

        }
    }
}