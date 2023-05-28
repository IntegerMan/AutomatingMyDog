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
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {

        public static Author UserAuthor = new Author("User");
        public static Author DogOSAuthor = new Author("DogOS");

        public Chat()
        {
            InitializeComponent();

            chat.CurrentAuthor = UserAuthor;
        }

        private void RadChat_SendMessage(object sender, SendMessageEventArgs e)
        {
            AppViewModel appVm = (AppViewModel)DataContext;
            
            // Indicate to the user that DogOS is processing the message
            chat.TypingIndicatorVisibility = Visibility.Visible;


            // Send the message over
            TextMessage message = (TextMessage)e.Message;
            appVm.ChatText = message.Text;
            appVm.SendMessageCommand.Execute(e.Message);

            // Don't have it show up automatically in the control
            e.Handled = true;
        }
    }
}
