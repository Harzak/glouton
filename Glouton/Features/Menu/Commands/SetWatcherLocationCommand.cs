using Glouton.Interfaces;
using Glouton.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Features.Menu.Commands;

internal class SetWatcherLocationCommand : IMenuCommand
{
    private readonly ISettingsService _settingsService;

    public SetWatcherLocationCommand(ISettingsService settingsService)
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