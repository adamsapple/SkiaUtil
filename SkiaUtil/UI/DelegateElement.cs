using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;

using SkiaUtil.Core;
using SkiaUtil.Extension;

namespace SkiaUtil.UI
{
    public delegate void SKCanvasPaintDelegate(SKCanvas canvas);

    /// <summary>
    /// 描画処理をLambdaで与えることができるElement.
    /// </summary>
    public class DelegateElement : DrawableElement
    {
        class InnerDrawable : IDrawable
        {
            internal DelegateElement outer;

            public float Width  => 0;
            public float Height => 0;

            public SKPaint PaintOption { get; set; }

            public void Paint(SKCanvas canvas) => outer?.PaintFunctions?.Invoke(canvas);
        }

        public SKCanvasPaintDelegate PaintFunctions;

        public DelegateElement() : base(new InnerDrawable())
        {
            if (Drawable is InnerDrawable inner)
            {
                inner.outer = this;
            }
        }
    }
}
