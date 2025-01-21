using Gui.Services;

namespace Gui.Elements
{
    public class Layer : ContainerElement
    {
        public Layer(IGuiServices guiServices) : base(guiServices)
        {
            Bounds = guiServices.GameBounds;
        }

        private View _parent;
        public View Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent?.RemoveLayer(this);
                    _parent = value;
                    _parent?.AddLayer(this);
                }
            }
        }

        public bool IsModal { get; set; }

        public virtual void OnShow() { }

        public virtual void OnHide() { }
    }
}