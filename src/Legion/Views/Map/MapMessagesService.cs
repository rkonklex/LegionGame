using System.Collections.Generic;
using Gui.Services;
using Legion.Localization;
using Legion.Model;
using Legion.Model.Types;
using Legion.Views.Map.Layers;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Map
{
    public class MapMessagesService : IMessagesService
    {
        private readonly ModalLayer _messagesLayer;
        private readonly IGuiServices _guiServices;
        private readonly ITexts _texts;

        public MapMessagesService(ModalLayer messagesLayer,
            IGuiServices guiServices,
            ITexts texts)
        {
            _messagesLayer = messagesLayer;
            _guiServices = guiServices;
            _texts = texts;
        }

        public void ShowMessage(Message message)
        {
            var args = new object[message.MapObjects.Count - 1];
            for (var i = 0; i < message.MapObjects.Count - 1; i++)
            {
                args[i] = message.MapObjects[i + 1].Name;
            }

            var messageKey = "message." + message.Type.ToString();
            var text = _texts.Get(messageKey, args);
            var title = message.MapObjects[0].Name;
            var image = GetImage(message.Type, message.MapObjects[0]);

            _messagesLayer.ShowMessage(title, text, image, message.OnClose);
        }

        private Texture2D GetImage(MessageType messageType, MapObject mapObject)
        {
            var imagesStore = _guiServices.ImagesStore;
            try
            {
                return imagesStore.GetImage("event." + messageType.ToString());
            }
            catch (KeyNotFoundException ex)
            {
                return mapObject switch
                {
                    Army army => imagesStore.GetImages("army.windowUsers")[army.Owner.Id - 1],
                    City city => imagesStore.GetImages("city.windows")[city.WallType],
                    _ => throw ex
                };
            }
        }
    }
}