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
    private string _searchText;
    private readonly List<LogEntry> _logEntries;

    public ObservableCollection<LogEntry> FilteredLogEntries { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    public ICommand ClearLogsCommand { get; }

    public LogViewModel(ILoggingService loggingService)
    {
        _logger = loggingService;
        _searchText = string.Empty;
        _logEntries = [];
        this.FilteredLogEntries = new ObservableCollection<LogEntry>();
        this.ClearLogsCommand = new ActionRelayCommand(ClearLogs);

        _logger.LogEntryAdded += OnLogEntryAdded;
    }

    private void OnLogEntryAdded(object? sender, LogEntryEventArgs arg)
    {
        _logEntries.Add(arg.LogEntry);
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        FilteredLogEntries.Clear();

        IEnumerable<LogEntry> filtered = _logEntries.Where(log =>
        {
            if (!string.IsNullOrEmpty(SearchText) && !log.Message.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        });

        foreach (LogEntry log in filtered)
        {
            FilteredLogEntries.Add(log);
        }
    }

    private void ClearLogs()
    {
        _logger.ClearLogs();
        _logEntries.Clear();
        FilteredLogEntries.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        _logger.LogEntryAdded -= OnLogEntryAdded;
        base.Dispose(disposing);
    }
}

