using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RadTabbedWindow
    {
        public MainWindow()
        {
            this.DataContext = new AppViewModel();

            InitializeComponent();
        }

        public object SpeakText { get; private set; }

        private void RadTabbedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AppViewModel vm = (AppViewModel)DataContext;
            vm.RegisterMessage(new AppMessage("Welcome to DogOS", MessageSource.DogOS) { SpeakText = "Welcome to doggos" });
        }
    }
}
