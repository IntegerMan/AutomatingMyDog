using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
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
            string? imageFile = files.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(imageFile))
            {
                AnalyzeImage(imageFile);
            }
        }
    }

    private void AnalyzeImage(string imageFilePath)
    {
        AppViewModel appVm = (AppViewModel)DataContext;

        // TODO: Indicate to the user that DogOS is processing the message
        // chat.TypingIndicatorVisibility = Visibility.Visible;

        try {
            appVm.RegisterMessage(new AppMessage($"Analyze Image File", MessageSource.User) {
                ImagePath = imageFilePath
            });
        }
        catch (NotSupportedException ex) 
        {
            const string ImageErrorMessage = "That image doesn't appear to be valid";
            appVm.RegisterMessage(new ChatMessageViewModel($"{ImageErrorMessage}: {ex.Message}", Chat.ErrorAuthor, "That image doesn't appear to be valid"));
        }
    }

    private void btnSendPhoto_Click(object sender, RoutedEventArgs e)
    {
        RadOpenFileDialog openFileDialog = new()
        {
            Owner = this,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            RestoreDirectory = true
        };
        openFileDialog.ShowDialog();

        if (openFileDialog.DialogResult == true)
        {
            string fileName = openFileDialog.FileName;

            AnalyzeImage(fileName);
        }
    }
}
