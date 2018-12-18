using Xamarin.Forms;

namespace SkiaUtil.Core.TouchTracking
{
    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public TouchEffect() : base("MyCompany.TouchEffect")
        {
        }

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}
