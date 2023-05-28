using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels;

public class ChatMessageViewModel : ViewModelBase
{
    public ChatMessageViewModel(string message, Author author)
    {
        Message = message;
        Author = author;
        CreationDate = DateTime.Now;
    }

    public string Message { get; }
    public Author Author { get; }
    public bool IsFromUser => Author.Name == "User";
    public DateTime CreationDate { get; }
}
