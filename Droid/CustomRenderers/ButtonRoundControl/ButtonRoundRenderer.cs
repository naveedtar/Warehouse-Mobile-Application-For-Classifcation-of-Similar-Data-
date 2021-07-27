using System.ComponentModel;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using WarehouseHandheld.Droid.Utilities;
using WarehouseHandheld.Droid.CustomRenderers.ButtonRoundControl;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using View = Android.Views.View;
using WarehouseHandheld.Elements.ButtonRound;
using Android.Content;

[assembly: ExportRenderer(typeof(ButtonRound), typeof(ButtonRoundRenderer))]
namespace WarehouseHandheld.Droid.CustomRenderers.ButtonRoundControl
{
    public class ButtonRoundRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            var view = (ButtonRound)this.Element;

            if (view == null || Control == null)
                return;

            Control.SetSingleLine(true);
            Control.SetMaxLines(1);

            Control.SetPadding(view.LeftInnerPadding * (int)Resources.DisplayMetrics.Density,
                view.TopInnerPadding * (int)Resources.DisplayMetrics.Density,
                view.RightInnerPadding * (int)Resources.DisplayMetrics.Density,
                view.BottomInnerPadding * (int)Resources.DisplayMetrics.Density);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ButtonRound.IsRoundedLeftCornersProperty.PropertyName ||
                e.PropertyName == ButtonRound.IsRoundedRightCornersProperty.PropertyName)
                Invalidate();
        }

        protected override bool DrawChild(Canvas canvas, View child, long drawingTime)
        {
            var view = (ButtonRound)this.Element;

            if (view.CornerRadius == 0 && view.BorderRadius != 0)
            {
                view.CornerRadius = view.BorderRadius;
                view.BorderRadius = 0;
            }

            Android.Graphics.Color pressed = view.BackgroundColor.ToAndroid();

            if (view.BackgroundColor == Color.Transparent ||
                view.BackgroundColor == Color.White)
            {
                pressed = view.BorderColor.ToAndroid();

                int[][] states = new int[][]{
                    new int[] { -Android.Resource.Attribute.StatePressed, -Android.Resource.Attribute.StateFocused, -Android.Resource.Attribute.StateSelected },
                    new int[] { Android.Resource.Attribute.StatePressed },
                    new int[] { Android.Resource.Attribute.StateFocused },
                    new int[] { Android.Resource.Attribute.StateSelected }
                };

                int[] textColors = new int[] {
                    view.TextColor.ToAndroid().ToArgb(),
                    Color.White.ToAndroid().ToArgb(),
                    Color.White.ToAndroid().ToArgb(),
                    Color.White.ToAndroid().ToArgb()
                };

                Control.SetTextColor(new Android.Content.Res.ColorStateList(states, textColors));
            }
            else
            {
                Control.SetTextColor(view.TextColor.ToAndroid());
                pressed = UiUtils.ModifyColor(pressed, 0.8f);
            }

            Drawable bg = ShapeUtils.GenerateDrawable(view.BackgroundColor.ToAndroid(),
                pressed,
                view.BorderColor.ToAndroid(),
                DpToPixel(Context, 1),
                DpToPixel(Context, view.CornerRadius),
                view.IsRoundedLeftCorners,
                view.IsRoundedRightCorners);

            Control.SetBackground(bg);
            return base.DrawChild(canvas, child, drawingTime);
        }

        public int DpToPixel(Context context, float dp)
        {
            var resources = context.Resources;
            var metrics = resources.DisplayMetrics;

            try
            {
                return (int)(dp * ((int)metrics.DensityDpi / 160f));
            }
            catch (Java.Lang.NoSuchFieldError)
            {
                return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, metrics);
            }

        }
    }
}
