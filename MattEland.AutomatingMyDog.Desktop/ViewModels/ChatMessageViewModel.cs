using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels;

public class ChatMessageViewModel : ViewModelBase
{
    public ChatMessageViewModel(string message, Author author, string? speakText = null)
    {
        Message = message;
        Author = author;
        SpeakText = speakText;
        CreationDate = DateTime.Now;
    }    
    
    public ChatMessageViewModel(AppMessage message)
    {
        Message = message.Message;
        Author = Chat.GetAuthor(message.Source);
        CreationDate = DateTime.Now;
        SpeakText = message.SpeakText;
        ImagePath = message.ImagePath;
        if (!string.IsNullOrWhiteSpace(ImagePath))
        {
            ImageSource = new BitmapImage(new Uri(ImagePath));
        }
    }

    public string Message { get; }
    public Author Author { get; }
    public string? SpeakText { get; }

    public bool IsFromUser => Author.Name == "User";
    public DateTime CreationDate { get; }
    public ImageSource? ImageSource { get; init; }
    public string? ImagePath { get; init; }
}
