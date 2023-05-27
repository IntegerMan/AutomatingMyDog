using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        FluentPalette.LoadPreset(FluentPalette.ColorVariation.Dark);
        StyleManager.ApplicationTheme = new FluentTheme();

        new MainWindow().Show();
        base.OnStartup(e);
    }
}
