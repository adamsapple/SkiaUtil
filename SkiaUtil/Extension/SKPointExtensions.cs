using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;

namespace SkiaUtil.Extension
{
    internal static class SKPointExtensions
    {
        public static SKPoint Normalize(this SKPoint self)
        {
            SKPoint result = new SKPoint(1f, 0);
            var length  = self.X * self.X + self.Y * self.Y;
            if (length != 0f)
            {
                result.X = self.X / length;
                result.Y = self.Y / length;
            }

            return result;
        }

        public static float Dot(this SKPoint self, SKPoint other)
        {
            return  self.X * other.X + self.Y * other.Y;
        }
    }
}
