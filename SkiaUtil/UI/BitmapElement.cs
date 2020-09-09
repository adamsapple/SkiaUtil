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

            public SKPaint PaintOption { get; set; }

            public void Paint(SKCanvas canvas) => canvas.DrawBitmap(bitmap, -Width/2, -Height/2, PaintOption);
            public DrawableBitmap(SKBitmap bitmap) => this.bitmap = bitmap;
        }

        public BitmapElement(SKBitmap bitmap) : base(new DrawableBitmap(bitmap))
        {
        }
    }
}
