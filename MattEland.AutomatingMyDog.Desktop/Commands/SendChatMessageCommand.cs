using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.Commands;

public class SendChatMessageCommand : CommandBase
{
    private readonly AppViewModel _vm;

    public SendChatMessageCommand(AppViewModel vm)
    {
        this._vm = vm;
    }

    public override void Execute(object parameter)
    {
        // Create the message
        ChatMessageViewModel message = new(_vm.ChatText, true);

        // Send the message to the chat service
        _vm.RegisterMessage(message);

        // Clear the chat text
        _vm.ChatText = string.Empty;
    }
}
