using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Commands;
using MattEland.AutomatingMyDog.Desktop.Pages;
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
        _endpoint = Properties.Settings.Default.CogServicesEndpoint ?? "";
        _key = Properties.Settings.Default.CogServicesKey ?? "";
        _region = Properties.Settings.Default.CogServicesRegion ?? "";

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
        Properties.Settings.Default.CogServicesEndpoint = endpoint;
        Properties.Settings.Default.CogServicesKey = key;
        Properties.Settings.Default.CogServicesRegion = region;
        Properties.Settings.Default.Save();

        // Notify VMs that our settings have changed
        _speech.UpdateAzureSettings(this);
        _text.UpdateAzureSettings(this);
    }

    internal void RegisterMessage(AppMessage message) => RegisterMessage(new ChatMessageViewModel(message));

    internal void RegisterMessage(ChatMessageViewModel message)
    {
        _messages.Add(message);

        // If the message is from the user, send it on to Azure
        if (message.Author == Chat.UserAuthor)
        {
            // Send it on to Azure
            Text.RespondTo(message.Message);
        } 
        else if (message.Author == Chat.DogOSAuthor)
        {
            // Say it aloud
            Speech.Say(message.Message, message.SpeakText);
        }
    }

    internal void ShowMessage(string message, string header)
    {
        RadWindow.Alert(new DialogParameters()
        {
            Header = header,
            Content = message,
        });
    }

    internal void HandleError(Exception ex, string header, bool showErrorBox = true)
    {
        RegisterMessage(new AppMessage(ex.Message, MessageSource.Error));
        
        if (showErrorBox)
        {
            ShowMessage(ex.Message, header);
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

    public Guid LuisAppId => new Guid("490898ab-294d-45b2-8c51-d7787a613153");
    public string LuisSlotId => "Production";

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