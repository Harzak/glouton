using Glouton.Commands;
using Glouton.EventArgs;
using Glouton.Features.Loging;
using Glouton.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Glouton.ViewModels;

public class LogViewModel : BaseViewModel
{
    private readonly ILoggingService _logger;
 
    public ObservableCollection<LogEntry> LogEntries { get; private set; }

    public ICommand ClearLogsCommand { get; }

    public LogViewModel(ILoggingService loggingService)
    {
        _logger = loggingService;
        this.LogEntries = new ObservableCollection<LogEntry>();
        this.ClearLogsCommand = new ActionRelayCommand(ClearLogs);

        _logger.LogEntryAdded += OnLogEntryAdded;
    }

    private void OnLogEntryAdded(object? sender, LogEntryEventArgs arg)
    {
        LogEntries.Add(arg.LogEntry);
    }

    private void ClearLogs()
    {
        _logger.ClearLogs();
        LogEntries.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        _logger.LogEntryAdded -= OnLogEntryAdded;
        base.Dispose(disposing);
    }
}