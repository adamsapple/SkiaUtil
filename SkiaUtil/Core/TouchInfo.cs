using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaUtil.Core
{
    internal class TouchInfo
    {
        public SKPoint PreviousPoint { set; get; }

        public SKPoint NewPoint { set; get; }
    }
}
