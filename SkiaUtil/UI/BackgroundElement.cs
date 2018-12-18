using SkiaSharp;
using SkiaUtil.Core;
using SkiaUtil.TouchManipulation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaUtil.UI
{
    internal class BackgroundElement : DrawableElement
    {
        /// <summary>
        /// 描くものがない
        /// </summary>
        class NoDraw : IDrawable
        {
            public float Width => 0;
            public float Height => 0;
            public void Paint(SKCanvas canvas)
            {
            }
        }

        public BackgroundElement() : base(new NoDraw())
        {
            TouchManager.Mode = TouchManipulationMode.None;
        }

        /// <summary>
        /// 背景は絶対触れる
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool HitTest(SKPoint location)
        {
            return true;
        }

        /// <summary>
        /// 何も書かない
        /// </summary>
        /// <param name="canvas"></param>
        public override void Paint(SKCanvas canvas)
        {
            return;
        }
    }
}
