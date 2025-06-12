using Glouton.Interfaces;
using Glouton.Settings;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Glouton.Features.Menu.Commands;

internal class OpenFileWatcherLocationCommand : IMenuCommand
{
    private readonly ISettingsService _settingsService;

    public OpenFileWatcherLocationCommand(ISettingsService settingsService)
    {
        _settingsService = settingsService;
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
        if (Directory.Exists(folderPath))
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = @"c:\windows\explorer.exe",
                Arguments = folderPath
            };
            Process.Start(startInfo);
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