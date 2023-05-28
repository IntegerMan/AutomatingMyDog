using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            AppViewModel vm = ((AppViewModel)DataContext);
            txtKey.Password = vm.Key;
            txtEndpoint.Text = vm.Endpoint;
            txtRegion.Text = vm.Region;
        }
    }
}
