using Glouton.Features.FileManagement.FileWatcher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Glouton.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private FileWatcherService _watcher;

    public ObservableCollection<string> Files { get; }

    public HomeViewModel()
    {
        _watcher = new FileWatcherService();
        this.Files = [];

        _watcher.Created += (sender, e) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Files.Add(e.FullPath);
                base.OnPropertyChanged(nameof(Files));
            });

        };
        _watcher.Start(@"C:\Users\Aurelien\Desktop\glouton");
    }
}