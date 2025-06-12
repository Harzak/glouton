using Glouton.Commands;
using Glouton.Features.Menu.Commands;
using Glouton.Interfaces;
using System.Windows.Input;

namespace Glouton.ViewModels;

public class MenuViewModel : BaseViewModel
{
    public ICommand OpenWatcherLocationCommand { get; }
    public ICommand SetWatcherLocationCommand { get; }

    public MenuViewModel(IMenuCommandInvoker commandInvoker,
                         ISettingsService settingsService)
    {
        IMenuCommand openWatcherLocationMenuCommand = new OpenFileWatcherLocationCommand(settingsService);
        IMenuCommand setWatcherLocationMenuCommand = new SetWatcherLocationCommand(settingsService);

        this.OpenWatcherLocationCommand = new MenuRelayCommand(openWatcherLocationMenuCommand, commandInvoker);
        this.SetWatcherLocationCommand = new MenuRelayCommand(setWatcherLocationMenuCommand, commandInvoker);
    }
}
