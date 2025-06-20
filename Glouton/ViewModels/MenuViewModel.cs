using Glouton.Commands;
using Glouton.Features.Menu.Commands;
using Glouton.Interfaces;
using System.Windows.Input;

namespace Glouton.ViewModels;

public class MenuViewModel : BaseViewModel
{
    public ICommand OpenWatchedLocationCommand { get; }
    public ICommand SetWatchedLocationCommand { get; }

    public MenuViewModel(IMenuCommandInvoker commandInvoker,
                         ISettingsService settingsService,
                         IProcessFacade processFacade,
                         IDirectoryFacade directoryFacade)
    {
        IMenuCommand openWatchedLocationMenuCommand = new OpenWatchedLocationCommand(settingsService, processFacade, directoryFacade);
        IMenuCommand setWatchedLocationMenuCommand = new SetWatchedLocationCommand(settingsService);

        this.OpenWatchedLocationCommand = new MenuRelayCommand(openWatchedLocationMenuCommand, commandInvoker);
        this.SetWatchedLocationCommand = new MenuRelayCommand(setWatchedLocationMenuCommand, commandInvoker);
    }
}
