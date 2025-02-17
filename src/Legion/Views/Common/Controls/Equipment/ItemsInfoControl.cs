using Gui.Elements;
using Gui.Services;
using Legion.Localization;
using Legion.Model.Types;
using Microsoft.Xna.Framework;

namespace Legion.Views.Common.Controls.Equipment
{
    public class ItemsInfoControl : DrawableElement
    {
        private readonly ITexts _texts;

        public ItemsInfoControl(IGuiServices guiServices, ITexts texts) : base(guiServices)
        {
            Size = new Point(105, 55);
            _texts = texts;
        }

        public Item Item { get; set; }

        protected override void OnDraw()
        {
            //GuiServices.BasicDrawer.DrawBorder(Color.Black, Bounds.X, Bounds.Y + 5, Bounds.Width, Bounds.Height);
            GuiServices.BasicDrawer.DrawRectangle(Color.Black, Bounds.X + 6, Bounds.Y + 71, 98 - 6, 88 - 71);

            if (Item != null)
            {
                var itemType = _texts.Get(Item.Type.Type.ToString());
                var itemName = _texts.Get(Item.Type.Name);
                
                GuiServices.BasicDrawer.DrawText(Color.AntiqueWhite, Bounds.X + 10, Bounds.Y + 72, itemType);
                GuiServices.BasicDrawer.DrawText(Color.AntiqueWhite, Bounds.X + 72, Bounds.Y + 72, "W: " + Item.Type.Weight);
                GuiServices.BasicDrawer.DrawText(Color.AntiqueWhite, Bounds.X + 10, Bounds.Y + 81, itemName);
            }
            GuiServices.BasicDrawer.DrawBorder(Colors.ItemContainerBorderColor, Bounds.X + 5, Bounds.Y + 70, 95, 20);
        }
    }
}