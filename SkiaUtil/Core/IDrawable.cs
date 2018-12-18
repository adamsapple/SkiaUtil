using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;


namespace SkiaUtil.Core
{
    /// <summary>
    /// 実際に描画する対象のIF
    /// </summary>
    public interface IDrawable
    {
        /// <summary>幅 </summary>
        float Width { get; }

        /// <summary>高さ </summary>
        float Height { get; }
        
        /// <summary>描画 </summary>
        void Paint(SKCanvas canvas);
    }
}
