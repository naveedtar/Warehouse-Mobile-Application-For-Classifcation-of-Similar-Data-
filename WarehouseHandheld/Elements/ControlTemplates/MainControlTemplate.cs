using System;
using Xamarin.Forms;
using System.Net.Http.Headers;

namespace WarehouseHandheld.Elements.ControlTemplates
{
    public class MainControlTemplate : Grid
    {
        public MainControlTemplate()
        {
            InitializeGrid();

            AddNavigationBar();

            AddContentPresenter();
        }

        private void InitializeGrid()
        {
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.ColumnSpacing = 0;
            this.RowSpacing = 0;
            this.RowDefinitions.Add(new RowDefinition()
            {
                Height = 50
            });
            this.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Star
            });
        }

        private void AddNavigationBar()
        {
            //Navigation Bar Grid
            Grid barGrid = new Grid()
            {
                BackgroundColor = Color.Cyan,
                RowDefinitions = {
                    new RowDefinition() { Height = GridLength.Auto}
                },
                ColumnDefinitions = {
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() { Width = GridLength.Star }
                }
            };

            Button backButton = new Button() { Text = "Back", BackgroundColor = Color.Transparent, FontSize = 16, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
            backButton.Clicked += BackButton_Clicked; ;
            barGrid.Children.Add(backButton, 0, 0);

            Label titleLabel = new Label() { FontSize = 20, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            titleLabel.SetBinding(Label.TextProperty, new TemplateBinding("Title"));

            barGrid.Children.Add(titleLabel, 1, 0);


            StackLayout toolbarStack = new StackLayout() { Orientation = StackOrientation.Horizontal };
            toolbarStack.Children.Add(new Label { Text = "Add", VerticalOptions = LayoutOptions.Center });
            ActivityIndicator indicator = new ActivityIndicator() { IsRunning = true, HeightRequest = 30, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End };
            indicator.SetBinding(ActivityIndicator.IsVisibleProperty, new TemplateBinding("IsBusy"));
            toolbarStack.Children.Add(indicator);


            barGrid.Children.Add(toolbarStack, 2, 0);

            //Adding Bar Content
            this.Children.Add(barGrid, 0, 0);
        }

        private void AddContentPresenter()
        {
            //Content Presenter
            ScrollView scroll = new ScrollView();
            scroll.Content = new ContentPresenter { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            this.Children.Add(scroll, 0, 1);
        }

        async void BackButton_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
                await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
