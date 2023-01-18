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

        public override void Update()
        {
            var imageW = 8; // XXX: army flag is 8px wide, but image width is 16px
            var armyX = _army.X - imageW / 2;
            var armyY = _army.Y - _image.Height - 1;
            Bounds = new Rectangle(armyX, armyY, imageW, _image.Height);
        }

        public override void Draw()
        {
            GuiServices.BasicDrawer.DrawImage(_image, Bounds.X, Bounds.Y);
        }
    }
}