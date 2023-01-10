using System.Collections.Generic;
using Gui.Services;
using Legion.Localization;
using Legion.Model;
using Legion.Model.Types;
using Legion.Views.Map.Layers;

namespace Legion.Views.Map
{
    public class MapMessagesService : IMessagesService
    {
        private readonly ModalLayer _messagesLayer;
        private readonly IGuiServices _guiServices;
        private readonly ITexts _texts;
        private readonly Dictionary<MessageType, string> _dict;

        public MapMessagesService(ModalLayer messagesLayer,
            IGuiServices guiServices,
            ITexts texts)
        {
            _messagesLayer = messagesLayer;
            _guiServices = guiServices;
            _texts = texts;
            _dict = new Dictionary<MessageType, string>();

            LoadData();
        }

        private void LoadData()
        {
            _dict.Add(MessageType.FireInTheCity, "event.fire");
            _dict.Add(MessageType.EpidemyInTheCity, "event.epidemy");
            _dict.Add(MessageType.RatsInTheCity, "event.rats");
            _dict.Add(MessageType.ChaosWarriorsBurnedCity, "event.burnedCity");
            //TODO: provide correct images for below types:
            _dict.Add(MessageType.RiotInTheCity, "event.burnedCity");
            _dict.Add(MessageType.UserAttackCity, "event.burnedCity");
            _dict.Add(MessageType.UserCapturedCity, "event.burnedCity");
            _dict.Add(MessageType.ArmyDestroyedCapturingCity, "event.burnedCity");
            _dict.Add(MessageType.EnemyAttacksUserCity, "event.burnedCity");
            _dict.Add(MessageType.EnemyCapturedUserCity, "event.burnedCity");
            _dict.Add(MessageType.EnemyConqueredUserCity, "event.burnedCity");
            _dict.Add(MessageType.EnemyConqueredCity, "event.burnedCity");
            _dict.Add(MessageType.RiotInTheCityLost, "event.burnedCity");
            _dict.Add(MessageType.RiotInTheCitySuccess, "event.burnedCity");
            _dict.Add(MessageType.RiotInTheCityWithDefence, "event.burnedCity");
            _dict.Add(MessageType.UserAttacksArmy, "event.burnedCity");
            _dict.Add(MessageType.EnemyAttacksUserArmy, "event.burnedCity");
            _dict.Add(MessageType.ArmyDestroyed, "event.burnedCity");
            _dict.Add(MessageType.ArmyTrackedDownBeast, "event.burnedCity");
            _dict.Add(MessageType.ArmyEncounteredRabidWolves, "event.burnedCity");
            _dict.Add(MessageType.ArmyEncounteredBandits, "event.burnedCity");
            _dict.Add(MessageType.ArmyStuckInSwamp, "event.burnedCity");
            _dict.Add(MessageType.ArmyEncounteredForestTrolls, "event.burnedCity");
            _dict.Add(MessageType.ArmyEncounteredGargoyl, "event.burnedCity");
            _dict.Add(MessageType.ArmyEncounteredLoneKnight, "event.burnedCity");
            _dict.Add(MessageType.ArmyEncounteredCaveEntrance, "event.burnedCity");
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
            var imageType = _dict[message.Type];
            var image = _guiServices.ImagesStore.GetImage(imageType);

            _messagesLayer.ShowMessage(title, text, image, message.OnClose);
        }
    }
}