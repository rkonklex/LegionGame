using Gui.Elements;
using Gui.Services;
using Legion.Model.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Map.Controls
{
    public class ArmyElement : SelectableElement
    {
        private readonly Army _army;
        private readonly Texture2D _image;

        public ArmyElement(IGuiServices guiServices, Army army) : base(guiServices)
        {
            _army = army;

            var armyImages = GuiServices.ImagesStore.GetImages("army.users");
            _image = armyImages[_army.Owner.Id - 1];
        }

        public Army Army => _army;

        protected override void OnUpdate()
        {
            base.OnUpdate();
            var armyX = _army.X - _image.Width / 2;
            var armyY = _army.Y - _image.Height - 1;
            Bounds = new Rectangle(armyX, armyY, _image.Width, _image.Height);
        }

        protected override void OnDraw()
        {
            GuiServices.BasicDrawer.DrawImage(_image, Bounds.X, Bounds.Y);
        }
    }
}