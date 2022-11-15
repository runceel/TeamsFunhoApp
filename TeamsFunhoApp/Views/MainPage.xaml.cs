using Microsoft.UI.Xaml.Controls;

using TeamsFunhoApp.ViewModels;

namespace TeamsFunhoApp.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
