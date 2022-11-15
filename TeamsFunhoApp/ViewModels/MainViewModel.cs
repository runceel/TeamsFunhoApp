using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TeamsFunhoApp.Contracts.Services;
using TeamsFunhoApp.Messages;

namespace TeamsFunhoApp.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string _message = "";

    private readonly IFunhoSender _funhoSender;
    private readonly IMessenger _messenger;

    public MainViewModel(IFunhoSender funhoSender, IMessenger messenger)
    {
        _funhoSender = funhoSender;
        _messenger = messenger;
    }

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        var (success, reason) = await _funhoSender.SendAsync(Message);
        if (success)
        {
            Message = "";
        }
        else
        {
            _messenger.Send(new ErrorMessage(reason!));
        }
    }

    private bool CanSendMessage() => !string.IsNullOrWhiteSpace(_message);
}
