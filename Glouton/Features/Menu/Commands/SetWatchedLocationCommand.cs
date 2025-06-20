using Glouton.Interfaces;
using Microsoft.Win32;
using System;
using System.IO;

namespace Glouton.Features.Menu.Commands;

internal class SetWatchedLocationCommand : IMenuCommand
{
    private readonly ISettingsService _settingsService;

    public SetWatchedLocationCommand(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public bool CanExecute()
    {
        return true;
    }

    public void Execute()
    {
        string path = _settingsService.GetSettings().WatchedFilePath;

        OpenFolderDialog dialog = new()
        {
            Title = "Select a folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
        };
        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
        {
            dialog.InitialDirectory = path;
        }
        if (dialog.ShowDialog() == true)
        {
            path = dialog.FolderName;
        }

        _settingsService.UpdateSetting("WatchedFilePath", path);
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}