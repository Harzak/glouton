using Glouton.Interfaces;
using Glouton.Settings;
using System;
using System.Diagnostics;
using System.Windows;

namespace Glouton.Features.Menu.Commands;

internal class OpenWatchedLocationCommand : IMenuCommand
{
    private readonly ISettingsService _settingsService;
    private readonly IProcessFacade _process;
    private readonly IDirectoryFacade _directory;

    public OpenWatchedLocationCommand(ISettingsService settingsService, IProcessFacade process, IDirectoryFacade directory)
    {
        _settingsService = settingsService;
        _process = process;
        _directory = directory;
    }

    public bool CanExecute()
    {
        AppSettings settings = _settingsService.GetSettings();
        return !string.IsNullOrEmpty(settings.WatchedFilePath?.Trim());
    }

    public void Execute()
    {
        AppSettings settings = _settingsService.GetSettings();
        string folderPath = settings.WatchedFilePath;
        if (_directory.Exists(folderPath))
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = @"c:\windows\explorer.exe",
                Arguments = folderPath
            };
            _process.Start(startInfo);
        }
        else
        {
            MessageBox.Show($"The folder '{folderPath}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void Undo()
    {
        throw new NotSupportedException();
    }
}