﻿using MattEland.AutomatingMyDog.Desktop.Pages;
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

    public override void Execute(object? parameter = null)
    {
        // Create the message
        string messageText = _vm.ChatText;
        ChatMessageViewModel message = new(messageText, Chat.UserAuthor);

        // Send the message to the chat service
        _vm.RegisterMessage(message);

        // Clear the chat text
        _vm.ChatText = string.Empty;
    }
}
