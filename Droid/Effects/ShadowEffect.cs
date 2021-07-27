using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.OS;
using WarehouseHandheld.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("WarehouseHandheld")]
[assembly: ExportEffect(typeof(ShadowEffect), "ShadowEffect")]
namespace WarehouseHandheld.Droid.Effects
{
    public class ShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Container.Elevation = 40;
            }
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
        }


        private Drawable GetShadowBackground()
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetShape(ShapeType.Rectangle);
            shape.SetColor(Color.Lime.ToAndroid());
            return shape;
        }
    }
}
