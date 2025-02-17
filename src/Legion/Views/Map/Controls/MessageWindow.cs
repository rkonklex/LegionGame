using Gui.Elements;
using Gui.Input;
using Gui.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System;

namespace Legion.Views.Map.Controls
{
    public class MessageWindow : Window
    {
        protected const int DefaultWidth = 112;
        protected const int DefaultHeight = 120;

        protected Panel InnerPanel;
        protected Image ImageElement;
        protected TextBlock TextLabel;
        protected Label TargetLabel;

        public MessageWindow(IGuiServices guiServices) : base(guiServices)
        {
            InnerPanel = new Panel(guiServices);
            ImageElement = new Image(guiServices);
            TextLabel = new TextBlock(guiServices);
            TargetLabel = new Label(guiServices);

            UpdateBounds();

            AddElement(InnerPanel);
            AddElement(ImageElement);
            AddElement(TextLabel);
            AddElement(TargetLabel);
        }

        public Action<HandledEventArgs> Closing;

        protected override void OnMouseDown(MouseButtonEventArgs args)
        {
            base.OnMouseDown(args);

            if (args.Button == MouseButton.Left)
            {
                args.Handled = true;
                Closing?.Invoke(new HandledEventArgs());
                Close();
            }
        }

        public Texture2D Image
        {
            get => ImageElement.Data;
            set => ImageElement.Data = value;
        }

        public string Text
        {
            get => TextLabel.Text;
            set => TextLabel.Text = value;
        }

        public string TargetName
        {
            get => TargetLabel.Text;
            set => TargetLabel.Text = value;
        }

        private void UpdateBounds()
        {
            var width = DefaultWidth;
            var height = DefaultHeight;

            var x = (GuiServices.GameBounds.Width / 2) - (width / 2);
            var y = (GuiServices.GameBounds.Height / 2) - (height / 2);

            Bounds = new Rectangle(x, y, width, height);
            InnerPanel.Bounds = new Rectangle(x + 4, y + 4, width - 7, height - 6);
            ImageElement.Bounds = new Rectangle(x + 8, y + 8, 96, 60);
            TargetLabel.Bounds = new Rectangle(x + 12, y + 80, 88, 10);
            TextLabel.Bounds = new Rectangle(x + 10, y + 90, 88, 20);
        }
    }
}