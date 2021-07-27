using System;
using Android.Graphics;
using Android.Graphics.Drawables;
namespace WarehouseHandheld.Droid.Utilities
{
    public static class ShapeUtils
    {
        private static Drawable GenerateSelector(Drawable normal,
                                                 Drawable pressed)
        {
            StateListDrawable selector = new StateListDrawable();
            selector.AddState(new int[] { -Android.Resource.Attribute.StatePressed, -Android.Resource.Attribute.StateFocused, -Android.Resource.Attribute.StateSelected }, normal);
            selector.AddState(new int[] { Android.Resource.Attribute.StatePressed }, pressed);
            selector.AddState(new int[] { Android.Resource.Attribute.StateFocused }, pressed);
            selector.AddState(new int[] { Android.Resource.Attribute.StateSelected }, pressed);

            return selector;
        }

        private static Drawable GenerateShape(Color colorBg,
                                              Color colorStroke,
                                              int strokeThickness,
                                              float strokeRadius,
                                              bool isLeftRounded,
                                              bool isRightRounded)
        {
            GradientDrawable drawable = new GradientDrawable();
            drawable.SetColor(colorBg);
            drawable.SetStroke(strokeThickness, colorStroke);
            float leftRadius = isLeftRounded ? strokeRadius : 0;
            float rightRadius = isRightRounded ? strokeRadius : 0;
            drawable.SetCornerRadii((new float[] { leftRadius, leftRadius, rightRadius, rightRadius,
                rightRadius, rightRadius, leftRadius, leftRadius }));

            return drawable;
        }

        public static Drawable GenerateDrawable(Color bgNormal,
                                                Color bgPressed,
                                                Color strokeColor,
                                                int strokeThickness,
                                                float strokeRadius,
                                                bool isLeftRounded,
                                                bool isRightRounded)
        {

            Drawable pressed = GenerateShape(bgPressed, strokeColor, strokeThickness, strokeRadius, isLeftRounded, isRightRounded);
            Drawable normal = GenerateShape(bgNormal, strokeColor, strokeThickness, strokeRadius, isLeftRounded, isRightRounded);

            return GenerateSelector(normal, pressed);
        }
    }
}
