using AwaitableCoroutine;
using Legion.Model.Types;
using System.Linq;

namespace Legion.Model
{
    public interface IMessagesService
    {
        void ShowMessage(Message message);
    }

    public static class MessagesServiceExtensions
    {
        public static Coroutine ShowMessageAsync(this IMessagesService messagesService, MessageType type, MapObject mainObject, MapObject otherObject = null)
        {
            var message = new Message
            {
                Type = type,
                MainObject = mainObject,
                OtherObject = otherObject,
            };
            messagesService.ShowMessage(message);
            return message.MessageClosed;
        }
    }
}