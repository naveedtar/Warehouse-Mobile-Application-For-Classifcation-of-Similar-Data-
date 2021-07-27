using System;
using Color = Android.Graphics.Color;
using Xamarin.Forms;
using Android.Graphics;
namespace WarehouseHandheld.Droid.Utilities
{
    public static class UiUtils
    {
        public static Color ModifyColor(Color color, float factor)
        {
            int alpha = color.A;
            int max = Math.Max(color.R, Math.Max(color.G, color.B));

            int r = color.R != max ? (int)Math.Round(color.R * factor) : color.R;
            int g = color.G != max ? (int)Math.Round(color.G * factor) : color.G;
            int b = color.B != max ? (int)Math.Round(color.B * factor) : color.B;

            return Color.Argb(alpha,
                              Math.Min(r, 255),
                              Math.Min(g, 255),
                              Math.Min(b, 255));
        }

        public static Typeface GetTypeface(string fontFamily)
        {
            int index = fontFamily.LastIndexOf("#");
            string fontName = string.Empty;
            if (index > 0)
                fontName = fontFamily.Substring(0, index);
            return Typeface.CreateFromAsset(Forms.Context.Assets, fontName);
        }

    }
}
