﻿using MattEland.AutomatingMyDog.Core;
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
        private LanguageUnderstandingHelper? _luis;

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
                _luis = new LanguageUnderstandingHelper(appViewModel.Key, appViewModel.Endpoint, appViewModel.LuisAppId, appViewModel.LuisSlotId);
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            Update(appViewModel);
        }

        public void RespondTo(string message)
        {
            // Abort if the app is not configured
            if (_text == null || _luis == null)
            {
                AppMessage notConfigMessage = new AppMessage("The application settings have not been configured. Please configure those first and try again.", MessageSource.DogOS);
                appViewModel.RegisterMessage(notConfigMessage);
            }
            else
            {
                // Start with text analysis
                foreach (AppMessage response in _text.AnalyzeText(message))
                {
                    appViewModel.RegisterMessage(response);
                }

                // Move on to LUIS
                foreach (AppMessage response in _luis.AnalyzeText(message))
                {
                    appViewModel.RegisterMessage(response);
                }
            }

        }
    }
}