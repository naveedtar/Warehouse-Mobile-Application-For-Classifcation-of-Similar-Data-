using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WarehouseHandheld.Elements.RotationImageElement
{
    public class RotationImage : Image
    {
        public static readonly BindableProperty IsRotatingProperty =
            BindableProperty.Create(nameof(IsRotating), typeof(bool), typeof(RotationImage), true, BindingMode.TwoWay, propertyChanged: RotationPropertyChanged);

        private static void RotationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var image = (bindable as RotationImage);
            Animation rotation = new Animation(x => image.RotateTo(360,800));


            if ((bool)newValue)
                rotation.Commit(image, "RotateAnimation", 16, 2000, Easing.Linear, (v, c) => image.RotateTo(0, 0), () => true);

            else
                //image.AbortAnimation("RotateAnimation");

            Debug.WriteLine("IsRotationChanged");
        }

        private async Task RotateElement(VisualElement element, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                await element.RotateTo(360, 800, Easing.Linear);
                await element.RotateTo(0, 0); // reset to initial position
            }
        }

        public bool IsRotating
        {
            get { return (bool)GetValue(IsRotatingProperty); }
            set { SetValue(IsRotatingProperty, value); }
        }

        public RotationImage()
        {
        }
    }
}
