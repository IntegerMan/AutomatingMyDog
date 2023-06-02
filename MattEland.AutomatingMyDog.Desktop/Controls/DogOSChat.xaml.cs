using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.Controls;

/// <summary>
/// Interaction logic for DogOSChat.xaml
/// </summary>
public partial class DogOSChat : UserControl
{
    public DogOSChat()
    {
        InitializeComponent();

        chat.CurrentAuthor = Chat.GetAuthor(MessageSource.User);
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

    private void chat_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Note that you can have more than one file.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Assuming you have one file that you care about, pass it off to whatever
            // handling code you have defined.
            string? firstFile = files.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(firstFile))
            {
                AppViewModel appVm = (AppViewModel)DataContext;

                // TODO: Indicate to the user that DogOS is processing the message
                // chat.TypingIndicatorVisibility = Visibility.Visible;

                appVm.RegisterMessage(new AppMessage($"Analyze Image File", MessageSource.User)
                {
                    ImagePath = firstFile
                });
            }
        }
    }
}
