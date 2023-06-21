using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Commands;
using MattEland.AutomatingMyDog.Desktop.Pages;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        _voice = Properties.Settings.Default.Voice ?? "en-US-GuyNeural";
        _languageEndpoint = string.IsNullOrWhiteSpace(Properties.Settings.Default.LanguageEndpoint) ? null : new Uri(Properties.Settings.Default.LanguageEndpoint);
        _languageKey = Properties.Settings.Default.LanguageKey ?? "";
        _openAIEndpoint = string.IsNullOrWhiteSpace(Properties.Settings.Default.OpenAIEndpoint) ? null : new Uri(Properties.Settings.Default.OpenAIEndpoint);
        _openAIKey = Properties.Settings.Default.OpenAIKey ?? "";
        _useTextAnalyis = Properties.Settings.Default.UseTextAnalysis;
        _useObjectDetection = Properties.Settings.Default.UseObjectDetection;
        _useCLU = Properties.Settings.Default.UseCLU;
        _useImageCropping = Properties.Settings.Default.UseImageCrop;
        _useOpenAI = Properties.Settings.Default.UseOpenAI;
        _useSpeech = Properties.Settings.Default.UseSpeech;

        // Set Helper View Models
        _speech = new SpeechViewModel(this);
        _text = new TextViewModel(this);
        _vision = new VisionViewModel(this);

        // Set commands
        _sendMessageCommand = new SendChatMessageCommand(this);
        _listenCommand = new ListenToSpeechCommand(this);
    }

    public SendChatMessageCommand SendMessageCommand => _sendMessageCommand;
    public ListenToSpeechCommand ListenCommand => _listenCommand;

    public SpeechViewModel Speech => _speech;
    public TextViewModel Text => _text;
    public VisionViewModel ComputerVision => _vision;

    public ObservableCollection<ChatMessageViewModel> Messages => _messages;

    private bool _useSpeech;
    private bool _useOpenAI;
    private bool _useImageCropping;
    private bool _useObjectDetection;
    private bool _useCLU;
    private bool _useTextAnalyis;

    public string AppName => "DogOS";
    public string Author => "Matt Eland";
    public string Title => $"{AppName} by {Author}";
    public string Version => "KCDC 2023 Edition";

    internal void SaveSettings(string endpoint, string key, string region, string? voice, Uri? languageEndpoint, string? languageKey, Uri? openAIEndpoint, string? openAIKey, bool useSpeech, bool useOpenAI, bool useCropping, bool useObjectDetect, bool useCLU, bool useTextAnalysis)
    {
        // Change our global settings
        Endpoint = endpoint;
        Key = key;
        Region = region;
        Voice = voice ?? "en-US-GuyNeural";
        LanguageKey = languageKey;
        LanguageEndpoint = languageEndpoint;
        OpenAIKey = openAIKey;
        OpenAIEndpoint = openAIEndpoint;
        UseSpeech = useSpeech;
        UseOpenAI = useOpenAI;
        UseCLU = useCLU;
        UseTextAnalysis = useTextAnalysis;
        UseImageCropping = useCropping;
        UseObjectDetection = useObjectDetect;

        // Update the settings file
        Properties.Settings.Default.CogServicesEndpoint = endpoint;
        Properties.Settings.Default.CogServicesKey = key;
        Properties.Settings.Default.CogServicesRegion = region;
        Properties.Settings.Default.Voice = voice;
        Properties.Settings.Default.LanguageKey = languageKey;
        Properties.Settings.Default.LanguageEndpoint = languageEndpoint?.AbsoluteUri;
        Properties.Settings.Default.OpenAIKey = OpenAIKey;
        Properties.Settings.Default.OpenAIEndpoint = OpenAIEndpoint?.AbsoluteUri;
        Properties.Settings.Default.UseSpeech = useSpeech;
        Properties.Settings.Default.UseCLU = useCLU;
        Properties.Settings.Default.UseTextAnalysis = useTextAnalysis;
        Properties.Settings.Default.UseImageCrop = useCropping;
        Properties.Settings.Default.UseObjectDetection = useObjectDetect;
        Properties.Settings.Default.UseOpenAI = useOpenAI;
        Properties.Settings.Default.Save();

        // Notify VMs that our settings have changed
        _speech.UpdateAzureSettings(this);
        _text.UpdateAzureSettings(this);
    }

    internal void RegisterMessage(AppMessage message) => RegisterMessage(new ChatMessageViewModel(message));
    internal async Task RegisterMessageAsync(AppMessage message) => await RegisterMessageAsync(new ChatMessageViewModel(message));

    internal void RegisterMessage(ChatMessageViewModel message)
    {
        _ = RegisterMessageAsync(message);
    }

    internal async Task RegisterMessageAsync(ChatMessageViewModel message)
    {
        _messages.Add(message);

        // If the message is from the user, send it on to Azure
        if (message.Author == Chat.UserAuthor)
        {
            // Send it on to Azure based on if it's an image or text message
            if (message.ImagePath != null)
            {
                await ComputerVision.RespondToAsync(message.ImagePath);
            }
            else
            {
                await Text.RespondToAsync(message.Message);
            }
        } 
        else if (message.Author == Chat.DogOSAuthor || !string.IsNullOrEmpty(message.SpeakText))
        {
            // Say it aloud
            await Speech.SayAsync(message.Message, message.SpeakText);
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

    internal void HandleError(string message, string header, bool showErrorBox = true)
    {
        RegisterMessage(new AppMessage(message, MessageSource.Error));

        if (showErrorBox)
        {
            ShowMessage(message, header);
        }
    }

    internal void HandleError(Exception ex, string header, bool showErrorBox = true) => HandleError(ex.Message, header, showErrorBox);

    private string _endpoint;
    private Uri? _openAIEndpoint;
    private Uri? _languageEndpoint;
    private string _key;
    private string? _languageKey;
    private string? _openAIKey;
    private string _region;
    private string _voice;
    private string _chatText = "";
    private readonly SpeechViewModel _speech;
    private readonly TextViewModel _text;
    private readonly VisionViewModel _vision;
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

    public string? OpenAIKey {
        get => _openAIKey;
        set {
            _openAIKey = value;
            OnPropertyChanged(nameof(OpenAIKey));
            OnPropertyChanged(nameof(IsOpenAIConfigured));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public string? LanguageKey {
        get => _languageKey;
        set {
            _languageKey = value;
            OnPropertyChanged(nameof(LanguageKey));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public Uri? LanguageEndpoint {
        get => _languageEndpoint;
        set {
            _languageEndpoint = value;
            OnPropertyChanged(nameof(LanguageEndpoint));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public Uri? OpenAIEndpoint {
        get => _openAIEndpoint;
        set {
            _openAIEndpoint = value;
            OnPropertyChanged(nameof(OpenAIEndpoint));
            OnPropertyChanged(nameof(IsOpenAIConfigured));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public bool UseSpeech {
        get => _useSpeech;
        set {
            _useSpeech = value;
            OnPropertyChanged(nameof(UseSpeech));
        }
    }

    public bool UseOpenAI {
        get => _useOpenAI;
        set {
            _useOpenAI = value;
            OnPropertyChanged(nameof(UseOpenAI));
        }
    }

    public bool UseImageCropping {
        get => _useImageCropping;
        set {
            _useImageCropping = value;
            OnPropertyChanged(nameof(UseImageCropping));
        }
    }

    public bool UseObjectDetection {
        get => _useObjectDetection;
        set {
            _useObjectDetection = value;
            OnPropertyChanged(nameof(UseObjectDetection));
        }
    }


    public bool UseCLU {
        get => _useCLU;
        set {
            _useCLU = value;
            OnPropertyChanged(nameof(UseCLU));
        }
    }

    public bool UseTextAnalysis {
        get => _useTextAnalyis;
        set {
            _useTextAnalyis = value;
            OnPropertyChanged(nameof(UseTextAnalysis));
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

    public string Voice
    {
        get => _voice;
        set
        {
            _voice = value;
            OnPropertyChanged(nameof(Voice));
            OnPropertyChanged(nameof(IsConfigured));
        }
    }

    public bool IsConfigured => !string.IsNullOrEmpty(Key) && 
        !string.IsNullOrEmpty(Endpoint) && 
        !string.IsNullOrEmpty(Region) &&        
        !string.IsNullOrEmpty(LanguageKey) && 
        LanguageEndpoint != null;

    public bool IsOpenAIConfigured => !string.IsNullOrEmpty(OpenAIKey) && OpenAIEndpoint != null;

    public Dispatcher UIThread { get; }
}