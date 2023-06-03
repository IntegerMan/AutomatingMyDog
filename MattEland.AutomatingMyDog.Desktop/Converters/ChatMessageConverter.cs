using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Linq;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.Converters;

public class ChatMessageConverter : IMessageConverter
{
    public MessageBase ConvertItem(object item)
    {
        ChatMessageViewModel messageModel = (ChatMessageViewModel)item;

        if (messageModel.Items != null && messageModel.Items.Any())
        {
            string itemText = messageModel.Items.Select(m => "- " + m).Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
            if (messageModel.ImageSource != null)
            {
                return new ImageCardMessage(messageModel.Author, messageModel.ImageSource, messageModel.CreationDate)
                {
                    Title = messageModel.Message,
                    Text = itemText,
                    ImageDisplayMode = messageModel.UseLandscapeLayout
                                        ? ImageDisplayMode.Thumbnail
                                        : ImageDisplayMode.Stretch,
                    CardOrientation = messageModel.UseLandscapeLayout 
                                        ? CardOrientation.Landscape 
                                        : CardOrientation.Portrait,
                };
            }
            else
            {
                return new CardMessage(messageModel.Author, messageModel.CreationDate)
                {
                    Title = messageModel.Message,
                    Text = itemText,
                };
            }
        }
        else if (messageModel.ImageSource != null)
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
