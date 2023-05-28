using MattEland.AutomatingMyDog.Desktop.Pages;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.Converters;

public class ChatMessageConverter : IMessageConverter
{
    public MessageBase ConvertItem(object item)
    {
        var messageModel = (ChatMessageViewModel)item;
        return new TextMessage(messageModel.Author, messageModel.Message, messageModel.CreationDate);
    }

    public object ConvertMessage(MessageBase message)
    {
        TextMessage textMessage = (TextMessage)message;
        return new ChatMessageViewModel(textMessage.Text, textMessage.Author);
    }
}
