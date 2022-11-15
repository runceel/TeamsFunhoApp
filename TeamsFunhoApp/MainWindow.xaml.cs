using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Threading;
using TeamsFunhoApp.Contracts.Services;
using TeamsFunhoApp.Helpers;
using TeamsFunhoApp.Messages;
using Windows.Foundation;
using Windows.UI.Popups;

namespace TeamsFunhoApp;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ErrorMessage>(this, ErrorMessageHandler);
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
        Observable.FromEventPattern<TypedEventHandler<object, WindowSizeChangedEventArgs>, WindowSizeChangedEventArgs>(
            h => SizeChanged += h,
            h => SizeChanged -= h)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizaitonContext.Current is null."))
            .Subscribe(async e =>
            {
                await App.GetService<IAppSettingsService>().SetWindowSizeAsync(
                    (int)e.EventArgs.Size.Width,
                    (int)e.EventArgs.Size.Height);
            });
    }

    private async void ErrorMessageHandler(object _, ErrorMessage errorMessage)
    {
        var dialog = new ContentDialog()
        {
            XamlRoot = Content.XamlRoot,
            Title = "ErrorDialogTitle".GetLocalized(),
            Content = errorMessage.Message,
            CloseButtonText = "CloseButtonText".GetLocalized(),
        };
        await dialog.ShowAsync();
    }
}
