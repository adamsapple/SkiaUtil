using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaUtil.Core.TouchTracking;
using SkiaUtil.TouchManipulation;


namespace SkiaUtil.Core
{
    public class DrawableElement
    {
        protected IDrawable Drawable { get; set; }

        public SKPaint PaintOption 
        {
            get => Drawable.PaintOption;
            set => Drawable.PaintOption = value;
        }

        Dictionary<long, TouchInfo> touchDictionary =
            new Dictionary<long, TouchInfo>();

        public DrawableElement(IDrawable drawable)
        {
            this.Drawable = drawable;
            Matrix        = SKMatrix.CreateIdentity();

            TouchManager = new TouchManipulationManager
            {
                Mode = TouchManipulationMode.ScaleRotate
            };
        }

        internal TouchManipulationManager TouchManager { set; get; }
        public TouchManipulationMode TouchMode 
        {
            get => TouchManager.Mode;
            set => TouchManager.Mode = value;
        }

        public DrawableElement Parent { set; get; } = null;

        public SKMatrix ParentMatrix => Parent?.Matrix ?? SKMatrix.CreateIdentity();

        public SKMatrix ConcatedMatrix
        {
            get
            {
                SKMatrix matrix       = Matrix;
                SKMatrix parentMatrix = Parent?.ConcatedMatrix ?? SKMatrix.CreateIdentity();//ParentMatrix;
                //SKMatrix.PostConcat(ref matrix, ref parentMatrix);
                matrix = matrix.PostConcat(parentMatrix);

                return matrix;
            }
        }

        public SKMatrix WorldToLocalMatrix()
        {
            ConcatedMatrix.TryInvert(out var inv);

            return inv;
        }

        public SKMatrix LocalToWorldMatrix()
        {
            return ConcatedMatrix;
        }

        public SKPoint WorldToLocal(SKPoint point)
        {
            ConcatedMatrix.TryInvert(out var inv);

            return inv.MapPoint(point);
        }

        public SKPoint LocalToWorld(SKPoint point)
        {
            return ConcatedMatrix.MapPoint(point);
        }



        /// <summary>
        /// 姿勢
        /// </summary>
        public SKMatrix Matrix { set; get; }

        public float Width => Drawable.Width;
        public float Height => Drawable.Height;
        private bool _isTouchable = false;
        public bool IsTouchable
        {
            get => _isTouchable;
            set
            {
                if (_isTouchable == value)
                {
                    return;
                }
                _isTouchable = value;
                if (!_isTouchable)
                {
                    touchDictionary.Clear();
                }
            }
        }

        public bool IsVisible { get; set; } = true;

        public event EventHandler<ElementTouchEventArgs> TouchEvent;

        private void RaiseTouchEvent(SKTouchAction type)
        {
            TouchEvent?.Invoke(this, new ElementTouchEventArgs(this, type));
        }

        public virtual void Paint(SKCanvas canvas)
        {
            if (!IsVisible)
            {
                return;
            }

            canvas.Save();

            var matrix = Matrix;

            //var offs = SKMatrix.MakeTranslation(-Width / 2, -Height / 2);
            //SKMatrix.PreConcat(ref matrix, offs);

            canvas.Concat(ref matrix);

            Drawable.Paint(canvas);

            canvas.Restore();
        }

        /// <summary>
        /// 評価対象のlocationが自身と衝突しているかどうか
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual bool HitTest(SKPoint location)
        {
            if (!IsTouchable)
            {
                return false;
            }
            // Invert the matrix

            SKMatrix matrix = ConcatedMatrix; // Matrix;
            //SKMatrix parentMatrix = Parent?.Matrix ?? SKMatrix.MakeIdentity();
            //SKMatrix.PreConcat(ref matrix, ref parentMatrix);

            if (matrix.TryInvert(out SKMatrix inverseMatrix))
            {
                // Transform the point using the inverted matrix
                SKPoint transformedPoint = inverseMatrix.MapPoint(location);

                //Debug.WriteLine($"HitTest : ({transformedPoint.X},{transformedPoint.Y})");

                SKRect rect = new SKRect(0, 0, Drawable.Width, Drawable.Height);
                rect.Offset(-Drawable.Width / 2, -Drawable.Height / 2);
                return rect.Contains(transformedPoint);
            }
            return false;
        }

        public void ProcessTouchEvent(long id, SKTouchAction type, SKPoint location)
        {
            /// Touch位置に逆行列をかけておく
            {
                SKMatrix matrix = ParentMatrix;
                if (matrix.TryInvert(out SKMatrix inverseMatrix))
                {
                    // Transform the point using the inverted matrix
                    SKPoint transformedPoint = inverseMatrix.MapPoint(location);

                    location = transformedPoint;
                }
            }

            switch (type)
            {
                case SKTouchAction.Pressed:
                    if (!touchDictionary.ContainsKey(id))
                    {
                        touchDictionary.Add(id, new TouchInfo
                        {
                            PreviousPoint = location,
                            NewPoint = location
                        });
                    }
                    break;

                case SKTouchAction.Moved:
                    var info = touchDictionary[id];
                    info.NewPoint = location;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case SKTouchAction.Released:
                    touchDictionary[id].NewPoint = location;
                    Manipulate();
                    touchDictionary.Remove(id);
                    break;

                case SKTouchAction.Cancelled:
                    touchDictionary.Remove(id);
                    break;
            }
            // Touch Eventの通知.
            RaiseTouchEvent(type);
        }

        void Manipulate()
        {
            var infos = new TouchInfo[touchDictionary.Count];
            touchDictionary.Values.CopyTo(infos, 0);

            SKMatrix touchMatrix = SKMatrix.CreateIdentity();

            if (infos.Length == 1)
            {
                SKPoint prevPoint  = infos[0].PreviousPoint;
                SKPoint newPoint   = infos[0].NewPoint;
                SKPoint pivotPoint = Matrix.MapPoint(Drawable.Width / 2, Drawable.Height / 2);

                touchMatrix = TouchManager.OneFingerManipulate(prevPoint, newPoint, pivotPoint);
            }
            else if (infos.Length >= 2)
            {
                int pivotIndex     = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                SKPoint pivotPoint = infos[pivotIndex].NewPoint;
                SKPoint newPoint   = infos[1 - pivotIndex].NewPoint;
                SKPoint prevPoint  = infos[1 - pivotIndex].PreviousPoint;

                touchMatrix = TouchManager.TwoFingerManipulate(prevPoint, newPoint, pivotPoint);
            }

            SKMatrix matrix = Matrix;
            //SKMatrix.PostConcat(ref matrix, ref touchMatrix);
            matrix = matrix.PostConcat(touchMatrix);
            Matrix = matrix;
        }
    }
}
