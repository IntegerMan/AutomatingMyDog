using MattEland.AutomatingMyDog.Desktop.Commands;
using MattEland.AutomatingMyDog.Desktop.Properties;
using System;
using System.Collections.Generic;
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
        _text = new TextViewModel(this);

        // Set commands
        _sendMessageCommand = new SendChatMessageCommand(this);
        _listenCommand = new ListenToSpeechCommand(this);
    }

    public SendChatMessageCommand SendMessageCommand => _sendMessageCommand;
    public ListenToSpeechCommand ListenCommand => _listenCommand;

    public SpeechViewModel Speech => _speech;
    public TextViewModel Text => _text;

    public ObservableCollection<ChatMessageViewModel> Messages => _messages;

    public string AppName => "DogOS";
    public string Author => "Matt Eland";
    public string Title => $"{AppName} by {Author}";
    public string Version => "SciFiDevCon 2023 Edition";

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
        _text.UpdateAzureSettings(this);
    }

    internal void RegisterMessage(ChatMessageViewModel message)
    {
        _messages.Add(message);

        // If the message is from the user, send it on to Azure
        if (message.IsFromUser)
        {
            // Send it on to Azure
            IEnumerable<ChatMessageViewModel> responses = Text.RespondTo(message.Message);

            // Process the responses
            foreach (ChatMessageViewModel response in responses)
            {
                // TODO: Some of these should be spoken aloud while others shouldn't
                _messages.Add(response);
            }
        }
    }

    private string _endpoint;
    private string _key;
    private string _region;
    private string _chatText = "";
    private readonly SpeechViewModel _speech;
    private readonly TextViewModel _text;
    private readonly SendChatMessageCommand _sendMessageCommand;
    private readonly ListenToSpeechCommand _listenCommand;
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

    public Dispatcher UIThread { get; }
}