using Gui.Elements;
using System;
using System.ComponentModel;

namespace Gui.Input
{
    internal static class InputHelper
    {
        public static void PropagateEvent<ElementT, ArgsT>(ElementT element, ArgsT args, Action<ElementT, ArgsT> handler)
            where ElementT : DrawableElement
            where ArgsT : HandledEventArgs
        {
            while (element != null)
            {
                handler(element, args);
                if (args.Handled)
                {
                    break;
                }

                element = FindParent<ElementT>(element);
            }
        }

        public static ParentT FindParent<ParentT>(DrawableElement element)
            where ParentT : DrawableElement
        {
            while (element.Parent != null)
            {
                if (element.Parent is ParentT parent)
                {
                    return parent;
                }
                element = element.Parent;
            }
            return null;
        }
    }
}