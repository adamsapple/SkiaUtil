using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp.Views.Forms;

namespace SkiaUtil.Core
{
    public class ElementTouchEventArgs : EventArgs
    {
        public DrawableElement Element { get; }
        public SKTouchAction Type { get; }

        public ElementTouchEventArgs(DrawableElement element, SKTouchAction type)
        {
            Element = element;
            Type    = type;
        }
    }
}
