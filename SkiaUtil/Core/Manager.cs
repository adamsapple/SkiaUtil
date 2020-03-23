using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using SkiaUtil.Core.TouchTracking;
using SkiaUtil.Extension;
using SkiaUtil.UI;
using System;

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

        public bool IsAutoRedraw { get; set; } = true;


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
            if (child == root)
            {
                child.Parent = null;
                return;
            }
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

        public void Touch(object sender, SKTouchEventArgs args)
        {
            switch (sender)
            {
                case SKCanvasView cv:
                    {
                        TouchImpl(cv.CanvasSize, cv.InvalidateSurface, args);
                    }
                    break;
                case SKGLView gl:
                    {
                        TouchImpl(gl.CanvasSize, gl.InvalidateSurface, args);
                    }
                    break;
                case Layout<View> views:
                    {
                        views.Children
                            .OfType<SKCanvasView>()
                            .ForEach(x => TouchImpl(x.CanvasSize, x.InvalidateSurface, args));
                    }
                    break;
            }
        }

        private void TouchImpl(SKSize realSize, Action invalidateSurface, SKTouchEventArgs args)
        {
            //var pt = args.Location;
            SKPoint point = args.Location;
            //new SKPoint((float)(size.Width * pt.X / canvasView.Width),
            //            (float)(size.Height * pt.Y / canvasView.Height));

            //point = touchBg.Matrix.MapPoint(point);

            switch (args.ActionType)
            {
                case SKTouchAction.Pressed:
                    // 最初にTouchされたモノを調べる
                    var tgt = touchIdsDics.Keys
                        .Reverse()
                        //.OfType<DrawableElement>()
                        .Where(o => o.HitTest(point))
                        .FirstOrDefault();
                    if (tgt != null)
                    {
                        // 描画物にTouch
                        touchIdsDics[tgt].Add(args.Id);
                        tgt.ProcessTouchEvent(args.Id, args.ActionType, point);
                    }
                    else
                    {
                        // CanvasにTouch
                    }
                    break;

                case SKTouchAction.Moved:
                    {
                        bool isRepaint = false;
                        touchIdsDics.Keys
                            .Where(o => touchIdsDics[o].Contains(args.Id))
                            //.OfType<DrawableElement>()
                            .ForEach((o) => {
                                o.ProcessTouchEvent(args.Id, args.ActionType, point);
                                isRepaint = true;
                            });

                        if (isRepaint && IsAutoRedraw)
                        {
                            invalidateSurface();
                        }
                    }
                    break;

                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    {
                        bool isRepaint = false;
                        touchIdsDics.Keys
                            .Where(o => touchIdsDics[o].Contains(args.Id))
                            //.OfType<DrawableElement>()
                            .ForEach((o) => {
                                o.ProcessTouchEvent(args.Id, args.ActionType, point);
                                touchIdsDics[o].Remove(args.Id);
                                isRepaint = true;
                            });

                        if (isRepaint && IsAutoRedraw)
                        {
                            invalidateSurface();
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
            SKImageInfo info  = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas   = surface.Canvas;

            canvas.Save();

            canvas.Concat(ref matrix);

            touchIdsDics.Keys
                .OfType<DrawableElement>()
                .ForEach(el => el.Paint(canvas));

            canvas.Restore();
        }
    }
}
