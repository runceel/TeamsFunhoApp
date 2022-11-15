using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TeamsFunhoApp.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string _message = "";

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {

    }

    private bool CanSendMessage() => !string.IsNullOrWhiteSpace(_message);
}
