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
                List<AppMessage> results = (await _vision.AnalyzeImageAsync(imagePath, _vm.UseImageCropping, _vm.UseObjectDetection)).ToList();

                // Respond to results
                foreach (AppMessage message in results)
                {
                    if (_vm is {IsOpenAIConfigured: true, UseOpenAI: true} && message.Source == MessageSource.DogOS) {
                        continue;
                    }

                    await _vm.RegisterMessageAsync(message);

                    if (_vm is {IsOpenAIConfigured: true, UseOpenAI: true} && message is {Message: "Captioning", Items: { }} && message.Items.Any()) {
                        string prompt = $"The user just showed you an image. Computer Vision describes the image as '{message.Items.First()}'. Describe this image in your own words to the user. If it has something a dog would be excited about, react to that.";
                        string openAiText = _vm.Text.GetReplyFromPrompt(prompt);
                        await _vm.RegisterMessageAsync(new AppMessage(openAiText, MessageSource.DogOS));
                    }

                }
            }

        }
    }
}