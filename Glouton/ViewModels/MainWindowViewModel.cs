using Glouton.Interfaces;

namespace Glouton.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    public MenuViewModel MenuViewModel { get; }
    public FileDetectionControlsViewModel FileDetectionControlsViewModel { get; }
    public LogViewModel LogViewModel { get; }
    public GloutonViewModel GloutonViewModel { get; }

    public MainWindowViewModel(
        IGlouton glouton,
        IFileDetection fileDetection,
        IMenuCommandInvoker commandInvoker,
        ISettingsService settingsService,
        IProcessFacade processFacade,
        IDirectoryFacade directoryFacade,
        ILoggingService logger)
    {
        //TODO Inject Viewmodel
        this.MenuViewModel = new MenuViewModel(commandInvoker, settingsService, processFacade, directoryFacade);
        this.FileDetectionControlsViewModel = new FileDetectionControlsViewModel(fileDetection, settingsService);
        this.LogViewModel = new LogViewModel(logger);
        this.GloutonViewModel = new GloutonViewModel(glouton);
    }
}