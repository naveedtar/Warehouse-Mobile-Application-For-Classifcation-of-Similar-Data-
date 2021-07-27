using System;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using WarehouseHandheld.ViewModels;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ValueConverters;
using WarehouseHandheld.Helpers;

namespace WarehouseHandheld.Views.Base.Popup
{
    public class PopupBase : PopupPage
    {
        public BaseViewModel ViewModel => BindingContext as BaseViewModel;

        public static readonly BindableProperty ShowSaveButtonProperty = BindableProperty.Create("ShowSaveButton", typeof(bool), typeof(PopupBase), false);


        public static readonly BindableProperty SaveButtonTextProperty = BindableProperty.Create("SaveButtonText", typeof(string), typeof(PopupBase), AppStrings.Done);

        public static readonly BindableProperty SaveButtonTextColorProperty = BindableProperty.Create("SaveButtonTextColor", typeof(Color), typeof(PopupBase), Color.Default);

        public static readonly BindableProperty ShowCancelButtonProperty = BindableProperty.Create("ShowCancelButton", typeof(bool), typeof(PopupBase), true);

        public static readonly BindableProperty DisableCancelPopProperty = BindableProperty.Create("DisableCancelPopProperty", typeof(bool), typeof(PopupBase), false);

        public static readonly BindableProperty CancelButtonTextProperty = BindableProperty.Create("CancelButtonText", typeof(string), typeof(PopupBase), AppStrings.Cancel);

        public static readonly BindableProperty CancelButtonTextColorProperty = BindableProperty.Create("CancelButtonTextColor", typeof(Color), typeof(PopupBase), Color.Default);

        public static readonly BindableProperty ShowHeaderProperty = BindableProperty.Create("ShowHeader", typeof(bool), typeof(PopupBase), true);

        public static readonly BindableProperty SaveButtonEnabledProperty = BindableProperty.Create("SaveButtonEnabled", typeof(bool), typeof(PopupBase), true);

        public PopupBase(bool animate = false)
        {
            ControlTemplate template = new ControlTemplate(typeof(PopupTemplate));
            HasSystemPadding = false;
            DependencyService.Get<ICrashLogHelper>().CrashLogs("PopUp Page Name : " + this.GetType().Name);
            this.ControlTemplate = template;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public bool ShowSaveButton
        {
            get { return (bool)GetValue(ShowSaveButtonProperty); }
            set { SetValue(ShowSaveButtonProperty, value); }
        }

        public string SaveButtonText
        {
            get { return (string)GetValue(SaveButtonTextProperty); }
            set { SetValue(SaveButtonTextProperty, value); }
        }

        public Color SaveButtonTextColor
        {
            get { return (Color)GetValue(SaveButtonTextColorProperty); }
            set { SetValue(SaveButtonTextColorProperty, value); }
        }

        public bool SaveButtonEnabled
        {
            get { return (bool)GetValue(SaveButtonEnabledProperty); }
            set { SetValue(SaveButtonEnabledProperty, value); }
        }

        #region SaveCommand
        public static BindableProperty SaveCommandProperty = BindableProperty.Create("SaveCommand", typeof(ICommand), typeof(PopupBase), null);
        public ICommand SaveCommand
        {
            get => (ICommand)this.GetValue(SaveCommandProperty);
            set { SetValue(SaveCommandProperty, value); }
        }
        #endregion SaveCommand


        public bool ShowCancelButton
        {
            get { return (bool)GetValue(ShowCancelButtonProperty); }
            set { SetValue(ShowCancelButtonProperty, value); }
        }

        public bool DisableCancelPop
        {
            get { return (bool)GetValue(DisableCancelPopProperty); }
            set { SetValue(DisableCancelPopProperty, value); }
        }

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }
            set { SetValue(CancelButtonTextProperty, value); }
        }


        public Color CancelButtonTextColor
        {
            get { return (Color)GetValue(CancelButtonTextColorProperty); }
            set { SetValue(CancelButtonTextColorProperty, value); }
        }


        public Action OnSaveClicked
        {
            get; set;
        }


        public Action OnCancelClicked
        {
            get; set;
        }

        public virtual void Initialize()
        { }
    }


    public class PopupTemplate : ContentView
    {
        StackLayout stack;


        public PopupTemplate()
        {
            SetPopupLayout();
            SetMainStack();
            AddContent();
        }

        void SetPopupLayout()
        {
            VerticalOptions = LayoutOptions.CenterAndExpand;
            HorizontalOptions = LayoutOptions.CenterAndExpand;
        }

        void SetMainStack()
        {
            stack = new StackLayout() { Spacing = 0, VerticalOptions = LayoutOptions.CenterAndExpand, BackgroundColor=Color.White, HorizontalOptions = LayoutOptions.CenterAndExpand, WidthRequest = App.ScreenWidth, HeightRequest = App.ScreenHeight };

            WidthRequest = App.ScreenWidth;
            HeightRequest = App.ScreenHeight;

        }

        void AddContent()
        {
            Button cancelButton = new Button { Text = AppStrings.Cancel, BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Center };
            cancelButton.SetBinding(IsVisibleProperty, new TemplateBinding("ShowCancelButton"));
            cancelButton.SetBinding(Button.TextProperty, new TemplateBinding("CancelButtonText"));
            cancelButton.SetBinding(Button.TextColorProperty, new TemplateBinding("CancelButtonTextColor"));

            Button saveButton = new Button { Text = AppStrings.Done, BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Center };
            saveButton.SetBinding(IsVisibleProperty, new TemplateBinding("ShowSaveButton"));
            saveButton.SetBinding(Button.TextProperty, new TemplateBinding("SaveButtonText"));
            saveButton.SetBinding(Button.TextColorProperty, new TemplateBinding("SaveButtonTextColor"));
            saveButton.SetBinding(Button.IsEnabledProperty, new TemplateBinding("SaveButtonEnabled"));

            Button rightButton = new Button { Text = "       ", BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Center };
            rightButton.SetBinding(IsVisibleProperty, new TemplateBinding("ShowSaveButton") { Converter = new InverseBooleanConverter() });

            Label titleLabel = new Label()
            {
                Margin = 10,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.End,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
            };
            titleLabel.SetBinding(Label.TextProperty, new TemplateBinding("Title"));

            StackLayout horizontalStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(0, 2, 0, 0),
                BackgroundColor = Color.FromHex("#f7f7f7")
            };
            horizontalStack.SetBinding(IsVisibleProperty, new TemplateBinding("ShowHeader"));
            horizontalStack.Children.Add(cancelButton);
            horizontalStack.Children.Add(titleLabel);
            horizontalStack.Children.Add(saveButton);
            horizontalStack.Children.Add(rightButton);
            //saveButton.IsEnabled = true;

            saveButton.Clicked += (sender, e) =>
            {
                try
                {
                    //((Button)sender).IsEnabled = false;
                    var popup = (PopupBase)(((Button)sender).Parent.Parent.Parent.Parent);
                    if (popup != null)
                    {
                        popup.SaveButtonEnabled = false;
                        popup.OnSaveClicked?.Invoke();
                        popup.SaveCommand?.Execute(null);
                        popup.SaveButtonEnabled = true;
                    }
                }
                catch
                {
                }
            };

            cancelButton.Clicked += (sender, e) =>
            {
                try
                {
                    var popup = (PopupBase)(((Button)sender).Parent.Parent.Parent.Parent);
                    if (popup == null)
                        PopupNavigation.PopAsync();
                    else
                    {
                        if(!popup.DisableCancelPop)
                            PopupNavigation.PopAsync();
                        popup.OnCancelClicked?.Invoke();
                    }
                }
                catch
                {
                }
            };

            var contentPresenter = new ContentPresenter() { VerticalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(horizontalStack);
            stack.Children.Add(contentPresenter);
            this.Content = stack;
        }
    }
}
