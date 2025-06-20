namespace Glouton.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    public MenuViewModel MenuViewModel { get; }
    public FileDetectionControlsViewModel FileDetectionControlsViewModel { get; }
    public LogViewModel LogViewModel { get; }
    public GloutonViewModel GloutonViewModel { get; }

    public MainWindowViewModel(MenuViewModel menuViewModel,
        FileDetectionControlsViewModel fileDetectionControlsViewModel,
        LogViewModel logViewModel,
        GloutonViewModel gloutonViewModel)
    {
        MenuViewModel = menuViewModel;
        FileDetectionControlsViewModel = fileDetectionControlsViewModel;
        LogViewModel = logViewModel;
        GloutonViewModel = gloutonViewModel;
    }
}