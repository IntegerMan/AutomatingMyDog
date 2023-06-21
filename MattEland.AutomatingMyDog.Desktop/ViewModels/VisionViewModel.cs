using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using SharpDX.Direct3D10;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (_vision == null) {
                AppMessage notConfigMessage = new("The application settings have not been configured. Please configure those first and try again.", MessageSource.DogOS);
                await _vm.RegisterMessageAsync(notConfigMessage);
            } else {
                // Open the file in a stream for analysis
                List<AppMessage> results = (await _vision.AnalyzeImageAsync(imagePath)).ToList();

                // Allow us to take creative liberties with the app
                AppMessage? dogMessage = results.FirstOrDefault(m => m.Source == MessageSource.DogOS);
                if (dogMessage != null && _vm.IsOpenAIConfigured) {

                    string prompt = dogMessage.Message;
                    AppMessage? captionMessage = results.FirstOrDefault(m => m.Message == "Captioning");

                    if (captionMessage != null && captionMessage.Items != null && captionMessage.Items.Any()) {
                        prompt = $"The user just uploaded an image. Computer Vision describes the image as '{captionMessage.Items.First()}'. Describe this image in your own words to the user. If it has something a dog would be excited about, react to that.";
                    }

                    dogMessage.Message = _vm.Text.GetReplyFromPrompt(prompt);
                }

                // Respond to results
                foreach (AppMessage message in results)
                {
                    await _vm.RegisterMessageAsync(message);
                }
            }

        }
    }
}