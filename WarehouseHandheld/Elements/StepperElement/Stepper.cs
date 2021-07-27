using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace WarehouseHandheld.Elements.StepperElement
{
    public class Stepper : ContentView
    {

        private Entry count = new Entry() { VerticalOptions = LayoutOptions.Center, FontSize = 18, Keyboard = Keyboard.Numeric, HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Center };
        private Button decrease = new Button() { Text = "-", VerticalOptions = LayoutOptions.Center, WidthRequest = 50 };
        private Button increase = new Button() { Text = "+", VerticalOptions = LayoutOptions.Center, WidthRequest = 50 };


        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(Stepper), 0.0, BindingMode.TwoWay, coerceValue: (bindable, value) =>
        {
            var stepper = (Stepper)bindable;
            return ((double)value).Clamp(stepper.Minimum, stepper.Maximum);
        }, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var stepper = (Stepper)bindable;
            EventHandler<ValueChangedEventArgs> eh = stepper.ValueChanged;
            if (eh != null)
                eh(stepper, new ValueChangedEventArgs((double)oldValue, (double)newValue));
            stepper.UpdateCount();
        });

        public static readonly BindableProperty MaximumProperty = BindableProperty.Create(nameof(Maximum), typeof(double), typeof(Stepper), 100000000.0, validateValue: (bindable, value) =>
        {
            var stepper = (Stepper)bindable;
            return (double)value > stepper.Minimum;
        }, coerceValue: (bindable, value) =>
        {
            var stepper = (Stepper)bindable;
            stepper.Value = stepper.Value.Clamp(stepper.Minimum, (double)value);
            return value;
        });

        public static readonly BindableProperty MinimumProperty = BindableProperty.Create(nameof(Minimum), typeof(double), typeof(Stepper), 0.0, validateValue: (bindable, value) =>
        {
            var stepper = (Stepper)bindable;
            return (double)value < stepper.Maximum;
        }, coerceValue: (bindable, value) =>
        {
            var stepper = (Stepper)bindable;
            stepper.Value = stepper.Value.Clamp((double)value, stepper.Maximum);
            return value;
        });

        public static readonly BindableProperty IncrementProperty = BindableProperty.Create(nameof(Increment), typeof(double), typeof(Stepper), 1.0);

        public static readonly BindableProperty IsEntryDisableProperty = BindableProperty.Create(nameof(IsEntryDisable), typeof(bool), typeof(Stepper), coerceValue: (bindable, value) => {
            var stepper = (Stepper)bindable;
            stepper.count.IsEnabled = !(bool)value;
            return value;
        });

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }


        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        public bool IsEntryDisable
        {
            get { return (bool)GetValue(IsEntryDisableProperty); }
            set { SetValue(IsEntryDisableProperty, value); }
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public Stepper()
        {
            count.TextChanged += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(count.Text))
                    Value = double.Parse(count.Text);
                else
                    Value = 0;
                count.Text = Value.ToString();
            };
            UpdateCount();
            decrease.Clicked += Decrease_Clicked;
            increase.Clicked += Increase_Clicked;
            Grid grid = new Grid() { HorizontalOptions = LayoutOptions.Fill };
            grid.RowDefinitions.Add(new RowDefinition() { Height = 50 });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

            grid.Children.Add(decrease, 0, 0);
            grid.Children.Add(count, 1, 0);
            grid.Children.Add(increase, 2, 0);

            this.Content = grid;
        }

        void Decrease_Clicked(object sender, EventArgs e)
        {
            Value = Value - Increment;
        }

        private void Increase_Clicked(object sender, EventArgs e)
        {
            Value = Value + Increment;
        }

        void UpdateCount()
        {
            count.Text = Value.ToString();
            count.IsEnabled = !IsEntryDisable;
        }

    }
}
