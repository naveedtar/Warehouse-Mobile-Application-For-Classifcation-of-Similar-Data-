using System;
using System.ComponentModel;
using CoreGraphics;
using WarehouseHandheld.iOS.Effects;
using WarehouseHandheld.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("WarehouseHandheld")]
[assembly: ExportEffect(typeof(WarehouseHandheld.iOS.Effects.ShadowEffect), "ShadowEffect")]

namespace WarehouseHandheld.iOS.Effects
{
    public class ShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Container.Layer.ShadowOffset = new CGSize(0, 0);
            UpdateSize();
            UpdateColor();
            UpdateOpacity();
        }

        protected override void OnDetached()
        {
            Container.Layer.ShadowOpacity = 0;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewEffects.HasShadowProperty.PropertyName)
            {
                UpdateOpacity();
            }
            else if (e.PropertyName == ViewEffects.ShadowColorProperty.PropertyName)
            {
                UpdateColor();
            }
            else if (e.PropertyName == ViewEffects.ShadowSizeProperty.PropertyName)
            {
                UpdateSize();
            }
        }

        private void UpdateOpacity()
        {
            Container.Layer.ShadowOpacity = (float)ViewEffects.GetShadowOpacity(Element);
        }

        private void UpdateColor()
        {
            var color = ViewEffects.GetShadowColor(Element);
            Container.Layer.ShadowColor = color.ToUIColor().CGColor;
        }

        private void UpdateSize()
        {
            Container.Layer.ShadowRadius = (nfloat)ViewEffects.GetShadowSize(Element);
        }
    }
}
