using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Linq;

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
            vm.SaveSettings(txtEndpoint.Text, txtKey.Password, txtRegion.Text, voice);

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
            txtEndpoint.Text = vm.Endpoint;
            txtRegion.Text = vm.Region;
            ddlVoice.SelectedIndex = vm.Speech.Voices.ToList().IndexOf(vm.Voice);
        }
    }
}
