using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.Commands;

public class ListenToSpeechCommand : CommandBase
{
    private readonly AppViewModel _vm;

    public ListenToSpeechCommand(AppViewModel vm)
    {
        this._vm = vm;
    }

    public override void Execute(object parameter)
    {
        string text = _vm.Speech.ListenForText();

        if (!string.IsNullOrWhiteSpace(text))
        {
            _vm.ChatText = text;
            _vm.SendMessageCommand.Execute();
        }
    }
}
