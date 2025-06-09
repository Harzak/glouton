using Glouton.Commands;
using Glouton.Features.FileManagement.FileWatcher;
using Glouton.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace Glouton.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    public ObservableCollection<string> Files { get;  }
    public LogViewModel LogViewModel { get; }
    public GloutonViewModel GloutonViewModel { get; }

    public MainWindowViewModel(IGlouton glouton, ILoggingService logger)
    {
        this.Files = [];
        this.LogViewModel = new LogViewModel(logger);
        this.GloutonViewModel = new GloutonViewModel(glouton);
 
        base.OnPropertyChanged(nameof(Files));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            
        }
        base.Dispose(disposing);
    }

    public ActionCommand ClearCommand => new ActionCommand(() =>
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Files.Clear();
            base.OnPropertyChanged(nameof(Files));
        });
    });
}

