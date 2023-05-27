using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels;

public class AppViewModel : ViewModelBase
{
    public AppViewModel()
    {
        UIThread = Dispatcher.CurrentDispatcher;
    }

    public string AppName => "DogOS";
    public string Author => "Matt Eland";
    public string Title => $"{AppName} by {Author}";
    public string Version => "SciFiDevCon 2023 Edition";

    public string FooterText => $"{AppName} {Version}";


    private void NotifyViewsChanged()
    {
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(FooterText));
    }


    private string _busyText = string.Empty;
    private double _busyProgress;
    private string? _endpoint;
    private string? _key;

    public string BusyText
    {
        get => _busyText;
        set
        {
            if (_busyText != value)
            {
                _busyText = value;
                NotifyViewsChanged();
            }
        }
    }

    public bool IsBusyIndeterminent => false;

    public double BusyProgress
    {
        get => _busyProgress;
        set
        {
            if (_busyProgress != value)
            {
                _busyProgress = value;
                OnPropertyChanged(nameof(BusyProgress));
            }
        }
    }

    public string? Endpoint
    {
        get => _endpoint;
        set
        {
            _endpoint = value;
            OnPropertyChanged(nameof(Endpoint));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public string? Key
    {
        get => _key;
        set
        {
            _key = value;
            OnPropertyChanged(nameof(Key));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public bool IsConfigured => !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(Endpoint);


    public bool IsBusy => !string.IsNullOrEmpty(_busyText);
    public Dispatcher UIThread { get; }
}