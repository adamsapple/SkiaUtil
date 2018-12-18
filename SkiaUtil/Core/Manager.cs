using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using SkiaUtil.Core.TouchTracking;
using SkiaUtil.Extension;
using SkiaUtil.UI;

namespace SkiaUtil.Core
{
    public class Manager
    {

        /// <summary>
        /// 
        /// </summary>
        Dictionary<DrawableElement, List<long>> touchIdsDics = new Dictionary<DrawableElement, List<long>>();

        /// <summary>
        /// 最背景に置かれるモノ
        /// </summary>
        private BackgroundElement root = new BackgroundElement()
        {
            IsTouchable = false
        };

        public bool IsEnableBgTouching
        {
            get => root.IsTouchable;
            set => root.IsTouchable = value;
        }

        public SKMatrix Matrix
        {
            get => root.Matrix;
            set => root.Matrix = value;
        }

        public Manager()
        {
            root.IsTouchable = IsEnableBgTouching;
            root.Parent = null;

            AddChild(root);   /// 背景を登録しておく
        }

        public void AddChild(DrawableElement child, DrawableElement parent = null)
        {
            if (touchIdsDics.ContainsKey(child))
            {
                return;
            }

            touchIdsDics.Add(child, new List<long>());
            if (parent == null)
            {
                child.Parent = root;
            }
        }

        public void RemoveChild(DrawableElement child)
        {
            if (!touchIdsDics.ContainsKey(child))
            {
                return;
            }

            touchIdsDics.Remove(child);
        }

        public void Touch(object sender, TouchActionEventArgs args)
        {
            switch (sender)
            {
                case SKCanvasView cv:
                    TouchImpl(cv, args);
                    break;
                case Layout<View> views:
                    views.Children
                        .OfType<SKCanvasView>()
                        .ForEach(x => TouchImpl(x, args));
                    break;
            }
        }

        private void TouchImpl(SKCanvasView canvasView, TouchActionEventArgs args)
        {
            Point pt = args.Location;
            SKPoint point =
                new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                            (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));

            //point = touchBg.Matrix.MapPoint(point);

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    // 最初にTouchされたモノを調べる
                    var tgt = touchIdsDics.Keys
                        .Reverse()
                        .OfType<DrawableElement>()
                        .Where(o => o.HitTest(point))
                        .FirstOrDefault();
                    if (tgt != null)
                    {
                        // 描画物にTouch
                        touchIdsDics[tgt].Add(args.Id);
                        if (tgt is DrawableElement el)
                        {
                            el.ProcessTouchEvent(args.Id, args.Type, point);
                        }
                    }
                    else
                    {
                        // CanvasにTouch
                    }
                    break;

                case TouchActionType.Moved:
                    {
                        bool isRepaint = false;
                        touchIdsDics.Keys
                            .Where(o => touchIdsDics[o].Contains(args.Id))
                            .OfType<DrawableElement>()
                            .ForEach((o) => {
                                o.ProcessTouchEvent(args.Id, args.Type, point);
                                isRepaint = true;
                            });

                        if (isRepaint)
                        {
                            canvasView.InvalidateSurface();
                        }
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    {
                        bool isRepaint = false;
                        touchIdsDics.Keys
                            .Where(o => touchIdsDics[o].Contains(args.Id))
                            .OfType<DrawableElement>()
                            .ForEach((o) => {
                                o.ProcessTouchEvent(args.Id, args.Type, point);
                                touchIdsDics[o].Remove(args.Id);
                                isRepaint = true;
                            });

                        if (isRepaint)
                        {
                            canvasView.InvalidateSurface();
                        }
                    }
                    break;
            }
        }

        public void Paint(object sender, SKPaintSurfaceEventArgs args)
        {
            var matrix = Matrix;
            PaintImpl(sender, args, ref matrix);
        }

        public void Paint(object sender, SKPaintSurfaceEventArgs args, ref SKMatrix matrix)
        {
            PaintImpl(sender, args, ref matrix);
        }

        public void PaintImpl(object sender, SKPaintSurfaceEventArgs args, ref SKMatrix matrix)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Save();

            canvas.Concat(ref matrix);

            touchIdsDics.Keys
                .OfType<DrawableElement>()
                .ForEach(el => el.Paint(canvas));

            canvas.Restore();
        }
    }
}
