using Glouton.Interfaces;

namespace Glouton.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    public MenuViewModel MenuViewModel { get; }
    public LogViewModel LogViewModel { get; }
    public GloutonViewModel GloutonViewModel { get; }

    public MainWindowViewModel(
        IGlouton glouton,
        IFileWatcherService fileWatcherService,
        IMenuCommandInvoker commandInvoker,
        ISettingsService settingsService,
        ILoggingService logger)
    {
        this.MenuViewModel = new MenuViewModel(commandInvoker, settingsService);
        this.LogViewModel = new LogViewModel(logger);
        this.GloutonViewModel = new GloutonViewModel(glouton);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {

        }
        base.Dispose(disposing);
    }
}