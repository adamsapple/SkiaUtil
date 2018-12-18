using SkiaSharp;
using SkiaUtil.Extension;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SkiaUtil.UI.Font
{
    public class BitmapFont
    {
        public SKBitmap Bitmap { get; set; }
        internal Dictionary<int, Abstractions.Char> Chars = new Dictionary<int, Abstractions.Char>();
        public float Height { get; private set; }
        public float Width { get; private set; }

        internal BitmapFont(List<Abstractions.Char> charList, SKBitmap bitmap)
        {
            Bitmap = bitmap;

            charList.ForEach(x =>
            {
                if(Chars.ContainsKey(x.Id)){
                    return;
                }

                Chars.Add(x.Id, x);
            });

            Width  = charList.Max(x => x.XAdvance);
            Height = charList.Max(x => x.Height);
        }

        public float MeasureText(string str)
        {
            var   dic    = Chars;
            float result = 0;

            str.ForEach(ch =>
            {
                Abstractions.Char c;

                if (!dic.TryGetValue((int)ch, out c))
                {
                    return;
                }
                result += c.XAdvance;
            });

            return result;
        }
    }

    public static class BitmapFontExtensions
    {
        public static void DrawBitmapFont(this SKCanvas canvas, BitmapFont bmFont, string str, float x, float y, SKPaint paint = null)
        {
            if(bmFont == null || bmFont.Bitmap == null)
            {
                return;
            }

            var dic = bmFont.Chars;
            
            str.ForEach(ch =>
            {
                int id = (int)ch;
                Abstractions.Char c;

                if (!dic.TryGetValue(id, out c))
                {
                    return;
                }

                SKRect src = new SKRect(0, 0, c.Width - 1, c.Height - 1);
                src.Offset(c.X, c.Y);

                SKRect dst = new SKRect(0, 0, c.Width - 1, c.Height - 1);
                dst.Offset(x + c.XOffset, y + c.YOffset);

                canvas.DrawBitmap(bmFont.Bitmap, src, dst);

                x += c.XAdvance;
            });
        }
    }
}
