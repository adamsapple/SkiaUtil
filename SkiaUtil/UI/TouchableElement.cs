using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;
using SkiaUtil.Core;

namespace SkiaUtil.UI
{
    public class TouchableElement : DrawableElement
    {
        class NullDraw : IDrawable
        {
            public float Width { get; set; }
            public float Height { get; set; }

            public void Paint(SKCanvas canvas)
            {
            }
        }

        public new float Width 
        {
            get => (Drawable as NullDraw).Width;
            set => (Drawable as NullDraw).Width = value;
        }
        public new float Height 
        {
            get => (Drawable as NullDraw).Height;
            set => (Drawable as NullDraw).Height = value;
        }

        public override void Paint(SKCanvas canvas) { }

        public TouchableElement(float w = 0, float h = 0):base(new NullDraw())
        {
            if(Drawable is NullDraw draw)
            {
                draw.Width  = w;
                draw.Height = h;
            }
        }

    }
}
