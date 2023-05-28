using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace MattEland.AutomatingMyDog.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        GreenPalette.LoadPreset(GreenPalette.ColorVariation.Dark);
        StyleManager.ApplicationTheme = new GreenTheme();

        new MainWindow().Show();
        base.OnStartup(e);
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // Display a message box with the error details and mark the error as handled
        RadWindow.Alert(new DialogParameters()
        {
            Header = "Unhandled Error",
            Content = e.Exception.Message,
        });
        e.Handled = true;

        // TODO: It'd be nice to log the error somewhere as well
    }
}
