﻿using Gui.Elements;
using Gui.Services;
using Legion.Model.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legion.Views.Common.Controls.Equipment
{
    public class ItemContainer : ClickableElement
    {
        private const int ItemMargin = 2;
        private const int DefaultWidth = 20;
        private const int DefaultHeight = 20;

        public ItemContainer(IGuiServices guiServices) : base(guiServices)
        {
            Size = new Point(DefaultWidth, DefaultHeight);
        }

        public bool IsTransparent { get; set; }

        private Item _item;
        public Item Item
        {
            get => _item;
            set
            {
                _item = value;
                Image = _item != null ? GuiServices.ImagesStore.GetImageByRealName(_item.Type.Img) : null;
            }
        }

        public Texture2D Image { get; private set; }

        protected override void OnDraw()
        {
            if (!IsTransparent)
            {
                GuiServices.BasicDrawer.DrawRectangle(Color.Black, Bounds);
            }
            GuiServices.BasicDrawer.DrawBorder(Colors.ItemContainerBorderColor, Bounds);

            if (Image != null)
            {
                GuiServices.BasicDrawer.DrawImage(Image, Bounds.X + ItemMargin, Bounds.Y + ItemMargin);
            }
        }
    }
}