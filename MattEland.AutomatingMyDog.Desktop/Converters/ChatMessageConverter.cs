using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.Converters;

public class ChatMessageConverter : IMessageConverter
{
    public MessageBase ConvertItem(object item)
    {
        var messageModel = (ChatMessageViewModel)item;

        if (messageModel.ImageSource != null)
        {
            return new ImageCardMessage(messageModel.Author, messageModel.ImageSource, messageModel.CreationDate)
            {
                Text = messageModel.Message
            };
        }
        else
        {
            return new TextMessage(messageModel.Author, messageModel.Message, messageModel.CreationDate);
        }
    }

    public object ConvertMessage(MessageBase message)
    {
        if (message is TextMessage textMessage)
        {
            return new ChatMessageViewModel(textMessage.Text, textMessage.Author);
        }
        else
        {
            throw new NotSupportedException("Cannot handle messages of type " + message.GetType().FullName);
        }
    }
}
