//using ext.GPS.UI.Satellite;
using SkiaSharp;
using SkiaUtil.Core;
using SkiaUtil.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkiaUtil.UI
{
    public class GroupElement : DrawableElement
    {
        class GroupDraw : IDrawable
        {
            internal GroupElement group;
            public float Width => 0;
            public float Height => 0;
            public void Paint(SKCanvas canvas)
            {
                group?.Children
                      .Values
                      .ForEach(x => x.Paint(canvas));
                //.Select(x =>
                //{
                //    x.Paint(canvas);
                //    return 0;
                //}).Sum();
                //.For(x => x.Paint(canvas));
            }
        }

        public SortedList<int, DrawableElement> Children = new SortedList<int, DrawableElement>();
        
        private readonly int Start     = 10000;
        private readonly int Increment = 100;


        public GroupElement() : base(new GroupDraw())
        {
            if (Drawable is GroupDraw draw)
            {
                draw.group = this;
            }
        }

        public override bool HitTest(SKPoint point)
        {
            var tgt = Children
                        .Values
                        .Reverse()
                        .OfType<DrawableElement>()
                        .Where(o => o.HitTest(point))
                        .FirstOrDefault();

            return (tgt != null);
        }

        public int AddChild(DrawableElement child)
        {
            /// 既に子供なら終了
            if (Children.ContainsValue(child))
            {
                //todo
                return Children.IndexOfValue(child);
            }

            /// 追加する子供のkeyを決定する
            var key = Children.Any() ? Children.Keys.Max() + Increment : Start;

            child.Parent = this;

            Children.Add(key, child);

            return key;
        }

        public void ClearChildren()
        {
            Children.Clear();
        }


        public void RemoveChild(int key)
        {
            /// 子供に存在しないなら終了
            if (!Children.ContainsKey(key))
            {
                return;
            }

            /// 子供削除
            Children.Remove(key);
        }

        public void RemoveChild(DrawableElement child)
        {
            /// 子供に存在しないなら終了
            if (!Children.ContainsValue(child))
            {
                return;
            }

            /// 子供削除
            var children = Children;
            children
                .Where(pair =>
                {
                    if (pair.Value != child)
                    {
                        return false;
                    }
                    child.Parent = null;
                    return true;
                })
                .ForEach((pair => RemoveChild(pair.Key)));
        }
    }
}
