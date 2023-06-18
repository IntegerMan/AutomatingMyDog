using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model.Themes;

namespace MattEland.AutomatingMyDog.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {

        SetTheme();

        new MainWindow().Show();
        base.OnStartup(e);
    }


    private void SetTheme() {
        // Set light or dark depending on system settings
        if (IsDarkTheme) {
            // The system is in dark theme
            GreenPalette.LoadPreset(GreenPalette.ColorVariation.Dark);
        } else {
            // The system is in light theme
            GreenPalette.LoadPreset(GreenPalette.ColorVariation.Light);
        }
        StyleManager.ApplicationTheme = new GreenTheme();
    }

    private bool IsDarkTheme {
        get {
            const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string RegistryValueName = "AppsUseLightTheme";

            try {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath)) {
                    object? registryValueObject = key?.GetValue(RegistryValueName);
                    if (registryValueObject == null) {
                        return false;
                    }

                    int registryValue = (int)registryValueObject;

                    return registryValue == 0;
                }
            }
            catch {
                // It's not worth crashing the app to determine our theme
                return false;
            }
        }
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
        StringBuilder sb = new();
        sb.AppendLine(e.Exception.Message);

#if DEBUG
        sb.AppendLine(e.Exception.StackTrace);
#endif

        // Display a message box with the error details and mark the error as handled
        RadWindow.Alert(new DialogParameters() {
            Header = "Unhandled Error",
            Content = sb.ToString(),
        });
        e.Handled = true;

        // TODO: It'd be nice to log the error somewhere as well
    }
}
