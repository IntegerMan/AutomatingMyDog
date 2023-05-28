using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels
{
    public class VisionViewModel : ViewModelBase
    {
        private AppViewModel _vm;
        private VisionHelper? _vision;

        public VisionViewModel(AppViewModel appViewModel)
        {
            _vm = appViewModel;

            Update(appViewModel);
        }

        private void Update(AppViewModel appViewModel)
        {
            if (appViewModel.IsConfigured)
            {
                _vision = new VisionHelper(appViewModel.Key, appViewModel.Endpoint);
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            Update(appViewModel);
        }

        public async Task RespondToAsync(string imagePath)
        {
            // Abort if the app is not configured
            if (_vision == null)
            {
                AppMessage notConfigMessage = new("The application settings have not been configured. Please configure those first and try again.", MessageSource.DogOS);
                await _vm.RegisterMessageAsync(notConfigMessage);
            }
            else
            {
                IEnumerable<AppMessage> results;

                // Open the file in a stream for analysis
                results = await _vision.AnalyzeImageAsync(imagePath);

                // Respond to results
                foreach (AppMessage message in results)
                {
                    await _vm.RegisterMessageAsync(message);
                }
            }

        }
    }
}