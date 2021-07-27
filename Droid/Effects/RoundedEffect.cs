using Android.Graphics;
using Android.Views;
using WarehouseHandheld.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(RoundedEffect), "RoundedEffect")]

namespace WarehouseHandheld.Droid.Effects
{
    public class RoundedEffect : PlatformEffect
    {

        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }

        public class ClipOutlineProvider : ViewOutlineProvider
        {
            private readonly int _cornerRadius = 0;

            public ClipOutlineProvider(int cornerRadius)
            {
                _cornerRadius = cornerRadius;
            }

            public override void GetOutline(Android.Views.View view, Outline outline)
            {
                outline.SetRoundRect(0, 0, view.Width,
                    view.Height, _cornerRadius);
            }
        }
    }
}
