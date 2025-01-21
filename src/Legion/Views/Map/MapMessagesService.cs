using System;
using System.Collections.Generic;
using Gui.Services;
using Legion.Localization;
using Legion.Model;
using Legion.Model.Types;
using Legion.Views.Map.Controls;
using Legion.Views.Map.Layers;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Map
{
    public class MapMessagesService : IMessagesService
    {
        private readonly Lazy<MapView> _mapView;
        private readonly IGuiServices _guiServices;
        private readonly ITexts _texts;

        public MapMessagesService(Lazy<MapView> mapView,
            IGuiServices guiServices,
            ITexts texts)
        {
            _mapView = mapView;
            _guiServices = guiServices;
            _texts = texts;
        }

        public void ShowMessage(Message message)
        {
            object[] args = null;
            if (message.OtherObject is not null)
            {
                args = [message.OtherObject.Name];
            }
            var messageKey = "message." + message.Type.ToString();
            
            var messageWindow = new MessageWindow(_guiServices)
            {
                TargetName = message.MainObject.Name,
                Text = _texts.Get(messageKey, args),
                Image = GetImage(message.Type, message.MainObject)
            };
            messageWindow.Closing += (_) => { message.NotifyMessageClosed(); };
            messageWindow.Open(_mapView.Value);
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