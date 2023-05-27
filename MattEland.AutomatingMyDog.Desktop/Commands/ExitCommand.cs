using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.Commands;

public class ExitCommand : CommandBase
{
    public override void Execute(object parameter)
    {
        RadWindowManager.Current.CloseAllWindows();
    }
}
