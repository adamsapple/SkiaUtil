using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Debug = XamLib.Diagnostics.Debug;

namespace SkiaUtil.UI.Font
{
    public class FontLoader
    {


        public static BitmapFont LoadFont(Stream fontxml, Stream png)
        {
            //var xml = XDocument.Load(fontxml);
            //var chars = xml.Element("font").Element("chars");
            //Debug.WriteLine(chars);

            var serializer = new XmlSerializer(typeof(SkiaUtil.UI.Font.Abstractions.Font));
            var font = (SkiaUtil.UI.Font.Abstractions.Font)serializer.Deserialize(fontxml);
            SKBitmap bitmap;

            using (SKManagedStream skStream = new SKManagedStream(png))
            {
                bitmap = SKBitmap.Decode(skStream);
            }

            return new BitmapFont(font.Chars.CharList, bitmap);
        }
    }
}
namespace SkiaUtil.UI.Font.Abstractions
{
    /// <summary>
    /// 個人情報
    /// </summary>
    [XmlRoot("font")]
    public class Font
    {
        [XmlElement("chars")]
        public Chars Chars { get; set; }
    }

    public class Chars
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("char")]
        public List<Char> CharList { get; set; }
    }

    public class Char
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("x")]
        public float X { get; set; }

        [XmlAttribute("y")]
        public float Y { get; set; }

        [XmlAttribute("width")]
        public float Width { get; set; }

        [XmlAttribute("height")]
        public float Height { get; set; }

        [XmlAttribute("xoffset")]
        public float XOffset { get; set; }

        [XmlAttribute("yoffset")]
        public float YOffset { get; set; }

        [XmlAttribute("xadvance")]
        public float XAdvance { get; set; }

        [XmlAttribute("page")]
        public int Page { get; set; }

        [XmlAttribute("chnl")]
        public int Channel { get; set; }
    }
}
