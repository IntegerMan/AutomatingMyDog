using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.Pages;

/// <summary>
/// Interaction logic for Chat.xaml
/// </summary>
public partial class Chat : UserControl
{

    public static Author UserAuthor = new Author("User");
    public static Author DogOSAuthor = new Author("DogOS");
    public static Author TextAnalysisAuthor = new Author("Text Analysis");
    public static Author ComputerVisionAuthor = new Author("Computer Vision");
    public static Author LuisAuthor = new Author("Language Understanding (LUIS)");
    public static Author ErrorAuthor = new Author("App Error Handler");

    public Chat()
    {
        InitializeComponent();

        chat.CurrentAuthor = UserAuthor;
    }

    internal static Author GetAuthor(MessageSource source)
    {
        return source switch
        {
            MessageSource.User => UserAuthor,
            MessageSource.DogOS => DogOSAuthor,
            MessageSource.LanguageUnderstanding => LuisAuthor,
            MessageSource.TextAnalytics => TextAnalysisAuthor,
            MessageSource.Error => ErrorAuthor,
            MessageSource.ComputerVision => ComputerVisionAuthor,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
        };
    }

    private void RadChat_SendMessage(object sender, SendMessageEventArgs e)
    {
        AppViewModel appVm = (AppViewModel)DataContext;
        
        // TODO: Indicate to the user that DogOS is processing the message
        // chat.TypingIndicatorVisibility = Visibility.Visible;

        // Send the message over
        TextMessage message = (TextMessage)e.Message;
        appVm.ChatText = message.Text;
        appVm.SendMessageCommand.Execute(e.Message);

        // Don't have it show up automatically in the control
        e.Handled = true;
    }
}
