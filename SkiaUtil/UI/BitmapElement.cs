using SkiaSharp;
using SkiaUtil.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaUtil.UI
{
    public class BitmapElement : DrawableElement
    {
        class DrawableBitmap : IDrawable
        {
            SKBitmap bitmap;

            public float Width => bitmap.Width;
            public float Height => bitmap.Height;
            public void Paint(SKCanvas canvas) => canvas.DrawBitmap(bitmap, 0, 0);
            public DrawableBitmap(SKBitmap bitmap) => this.bitmap = bitmap;
        }

        public BitmapElement(SKBitmap bitmap) : base(new DrawableBitmap(bitmap))
        {
        }
    }
}
