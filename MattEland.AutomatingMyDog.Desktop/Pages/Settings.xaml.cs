using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Linq;
using System;

namespace MattEland.AutomatingMyDog.Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void SaveSettingsClick(object sender, RoutedEventArgs e)
        {
            AppViewModel vm = (AppViewModel)DataContext;

            // A lot of the manual code in this View is due to PasswordBox not supporting data binding for security purposes
            // Because of that we need to get the password out manually and set it manually
            string voice = ddlVoice.SelectedItem?.ToString() ?? "en-US-GuyNeural";
            Uri.TryCreate(txtLanguageEndpoint.Text, UriKind.Absolute, out Uri? languageEndpoint);
            Uri.TryCreate(txtOpenAIEndpoint.Text, UriKind.Absolute, out Uri? openAIEndpoint);
            vm.SaveSettings(txtEndpoint.Text, txtKey.Password, txtRegion.Text, voice, languageEndpoint, txtLanguageKey.Password, openAIEndpoint, txtOpenAIKey.Password, toggleSpeech.IsChecked == true, toggleOpenAI.IsChecked == true, toggleImageCrop.IsChecked == true, toggleObjectDetect.IsChecked == true, toggleCLU.IsChecked == true, toggleTextAnalysis.IsChecked == true);

            if (vm.IsConfigured)
            {
                vm.RegisterMessage(new ChatMessageViewModel("Your settings have been saved.", Chat.DogOSAuthor));
            }
            else
            {
                // Tell the user we saved the settings since the audio won't work
                vm.ShowMessage("Your settings have been saved.", "Settings Saved");
            }

            // TODO: Navigate to the home page
        }

        private void CancelSettingsClick(object sender, RoutedEventArgs e)
        {
            ResetSettings();

            // TODO: Navigate to the home page
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ResetSettings();
        }

        private void ResetSettings()
        {
            // A lot of the manual code in this View is due to PasswordBox not supporting data binding for security purposes
            // Because of that we need to get the password out manually and set it manually. However, this also gives us the ability
            // to allow edits to the settings that don't propagate to the main VM until the user clicks save. This also allows us to
            // easily reset the edit state by just reloading the settings from the VM

            AppViewModel vm = ((AppViewModel)DataContext);
            txtKey.Password = vm.Key;
            txtLanguageKey.Password = vm.LanguageKey;
            txtLanguageEndpoint.Text = vm.LanguageEndpoint?.AbsoluteUri;
            txtEndpoint.Text = vm.Endpoint;
            txtRegion.Text = vm.Region;
            ddlVoice.SelectedIndex = vm.Speech.Voices.ToList().IndexOf(vm.Voice);
            txtOpenAIEndpoint.Text = vm.OpenAIEndpoint?.AbsoluteUri;
            txtOpenAIKey.Password = vm.OpenAIKey;
            toggleCLU.IsChecked = vm.UseCLU;
            toggleImageCrop.IsChecked = vm.UseImageCropping;
            toggleObjectDetect.IsChecked = vm.UseObjectDetection;
            toggleOpenAI.IsChecked = vm.UseOpenAI;
            toggleSpeech.IsChecked = vm.UseSpeech;
            toggleTextAnalysis.IsChecked = vm.UseTextAnalysis;
        }
    }
}
