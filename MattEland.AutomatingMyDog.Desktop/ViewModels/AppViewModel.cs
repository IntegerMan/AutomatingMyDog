using MattEland.AutomatingMyDog.Desktop.Commands;
using MattEland.AutomatingMyDog.Desktop.Properties;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels;

public class AppViewModel : ViewModelBase
{
    public AppViewModel()
    {
        UIThread = Dispatcher.CurrentDispatcher;

        // Load Settings
        _endpoint = Settings.Default.CogServicesEndpoint ?? "";
        _key = Settings.Default.CogServicesKey ?? "";
        _region = Settings.Default.CogServicesRegion ?? "";

        // Set Helper View Models
        _speech = new SpeechViewModel(this);

        // Set commands
        _sendMessageCommand = new SendChatMessageCommand(this);
    }

    public SendChatMessageCommand SendMessageCommand => _sendMessageCommand;

    public SpeechViewModel Speech => _speech;

    public ObservableCollection<ChatMessageViewModel> Messages => _messages;

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

    internal void SaveSettings(string endpoint, string key, string region)
    {
        // Change our global settings
        Endpoint = endpoint;
        Key = key;
        Region = region;

        // Update the settings file
        Settings.Default.CogServicesEndpoint = endpoint;
        Settings.Default.CogServicesKey = key;
        Settings.Default.CogServicesRegion = region;
        Settings.Default.Save();

        // Notify VMs that our settings have changed
        _speech.UpdateAzureSettings(this);
    }

    internal void RegisterMessage(ChatMessageViewModel message)
    {
        _messages.Add(message);

        // TODO: if the message is from the user, send it to Azure
    }

    private string _busyText = string.Empty;
    private double _busyProgress;
    private string _endpoint;
    private string _key;
    private string _region;
    private string _chatText = "";
    private readonly SpeechViewModel _speech;
    private readonly SendChatMessageCommand _sendMessageCommand;
    private readonly ObservableCollection<ChatMessageViewModel> _messages = new();

    public string ChatText
    {
        get => _chatText;
        set
        {
            _chatText = value;
            OnPropertyChanged(nameof(ChatText));
        }
    }

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

    public string Endpoint
    {
        get => _endpoint;
        set
        {
            _endpoint = value;
            OnPropertyChanged(nameof(Endpoint));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public string Key
    {
        get => _key;
        set
        {
            _key = value;
            OnPropertyChanged(nameof(Key));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public string Region
    {
        get => _region;
        set
        {
            _region = value;
            OnPropertyChanged(nameof(Region));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public bool IsConfigured => !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(Endpoint) && !string.IsNullOrEmpty(Region);

    public bool IsBusy => !string.IsNullOrEmpty(_busyText);
    public Dispatcher UIThread { get; }
}