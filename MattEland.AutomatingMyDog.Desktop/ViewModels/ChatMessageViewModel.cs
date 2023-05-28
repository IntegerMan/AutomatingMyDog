using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels;

public class ChatMessageViewModel : ViewModelBase
{
    public ChatMessageViewModel(string message, bool isFromUser)
    {
        Message = message;
        IsFromUser = isFromUser;
    }

    public string Message { get; }
    public bool IsFromUser { get; }
}
