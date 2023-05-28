using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

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
            vm.SaveSettings(txtEndpoint.Text, txtKey.Password, txtRegion.Text);

            if (vm.IsConfigured)
            {
                vm.Speech.Say("Your settings have been saved.");
            }

            // Tell the user we saved the settings
            RadWindow.Alert(new DialogParameters()
            {
                Header = "Settings Saved",
                Content = "Your settings have been saved",
            });

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
        }
    }
}
