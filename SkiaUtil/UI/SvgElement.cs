using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;

using SkiaUtil.Core;

namespace SkiaUtil.UI
{
    public class SvgElement : DrawableElement
    {
        public class DrawableSvg : IDrawable
        {
            SkiaSharp.Extended.Svg.SKSvg svg;

            public float Width =>  svg.Picture.CullRect.Width;
            public float Height => svg.Picture.CullRect.Height;
            public void Paint(SKCanvas canvas) => canvas.DrawPicture(svg.Picture);
            public DrawableSvg(SkiaSharp.Extended.Svg.SKSvg svg) => this.svg = svg;
        }

        public SvgElement(SkiaSharp.Extended.Svg.SKSvg svg) : base(new DrawableSvg(svg))
        {
        }
    }
}