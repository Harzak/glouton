using Glouton.Commands;
using Glouton.Features.FileManagement.FileWatcher;
using Glouton.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace Glouton.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private IFileWatcherService _watcher;

    public ObservableCollection<string> Files { get;  }
    public LogViewModel LogViewModel { get; }

    public MainWindowViewModel(IFileWatcherService watcher, ILoggingService logger)
    {
        _watcher = watcher;

        this.Files = [];
        this.LogViewModel = new LogViewModel(logger);

        _watcher.Created += (sender, e) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Files.Add(e.FullPath);
                base.OnPropertyChanged(nameof(Files));
            });

        };
        base.OnPropertyChanged(nameof(Files));

        _watcher.Start(@"C:\Users\Aurelien\Desktop\glouton");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _watcher?.Dispose();
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

