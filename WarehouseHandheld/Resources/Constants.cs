using System;
using Xamarin.Forms;
using Xamarin.Forms.DataGrid;

namespace WarehouseHandheld.Resources
{
    public static class Constants
    {
        // Start Sync Alert Constants
        public static string PalletTrackingSync = "PalletTrackingSync";
        // End Sync Alert Constants



        public static int GridRowHeight = 60;
        public static int GridHeaderHeight = 25;
        public static string[] PrintLineSeparator = { "|n|" };
        public static Color GridHeaderBackgroundColor = Color.FromHex("#00a9e7");
        public static Color GridHeaderTextColor = Color.White;
        public static ColorProvider GridColors = new ColorProvider();
        public static ColorProviderLocal GridColorsLocal = new ColorProviderLocal();
        public static int[] MinutesList = new int[] { 1, 2, 3, 4, 5, 10, 15, 30, 45, 60 };
        public static string ApiErrorMsg = "Something went wrong.Please contact admin.";
        public static int BarcodeHeight = 40;
        public static int BarcodeWidth = 4;
        public static int PosX = 10;
        public static int PosY = 20;

        public static void SetGridProperties(DataGrid grid)
        {
            grid.RowsBackgroundColorPalette = GridColors;
            grid.HeaderBackground = GridHeaderBackgroundColor;
            grid.HeaderTextColor = GridHeaderTextColor;
            grid.BorderThickness = new Thickness(0);
            grid.RowHeight = GridRowHeight;
            grid.VerticalOptions = LayoutOptions.FillAndExpand;
            grid.FontSize = 12;
        }

        public static void SetGridProperties(Xamarin.Forms.DataGridLocal.DataGrid grid)
        {
            grid.RowsBackgroundColorPalette = GridColorsLocal;
            grid.HeaderBackground = GridHeaderBackgroundColor;
            grid.HeaderLabelStyle = new Style(typeof(Label))
            {
                Setters = { new Setter { Property = Label.TextColorProperty, Value = GridHeaderTextColor } }
            };
            grid.BorderThickness = new Thickness(0);
            grid.RowHeight = GridRowHeight;
            grid.VerticalOptions = LayoutOptions.FillAndExpand;
            grid.FontSize = 12;
        }
    }

    public class ColorProvider : IColorProvider
    {
        public Color GetColor(int a, object o)
        {
            bool IsEven = a % 2 == 0;
            if (IsEven)
            {
                return Color.FromHex("#e0dfdf");
            }
            else
            {
                return Color.FromHex("#fcfcfc");
            }
        }
    }

    public class ColorProviderLocal : Xamarin.Forms.DataGridLocal.IColorProvider
    {
        public Color GetColor(int a, object o)
        {
            bool IsEven = a % 2 == 0;
            if (IsEven)
            {
                return Color.FromHex("#e0dfdf");
            }
            else
            {
                return Color.FromHex("#fcfcfc");
            }
        }
    }
}
