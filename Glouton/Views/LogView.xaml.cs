using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Glouton.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        private bool _isScrolling;

        public LogView()
        {
            InitializeComponent();
            this.DataContextChanged += LogView_DataContextChanged;
            this.Unloaded += LogView_Unloaded;
        }
        
        private void LogView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ViewModels.LogViewModel oldViewModel && 
                oldViewModel.LogEntries is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= FilteredLogEntries_CollectionChanged;
            }
            
            if (e.NewValue is ViewModels.LogViewModel newViewModel && 
                newViewModel.LogEntries is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += FilteredLogEntries_CollectionChanged;
            }
        }
        
        private void FilteredLogEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isScrolling)
            {
                return;
            }
                
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (logDataGrid.Items.Count > 0)
                {
                    try
                    {
                        _isScrolling = true;              
                        logDataGrid.Dispatcher.BeginInvoke((Action)(() => 
                        {
                            try
                            {
                                if (logDataGrid.Items.Count > 0)
                                {
                                    var lastIndex = logDataGrid.Items.Count - 1;
                                    logDataGrid.ScrollIntoView(logDataGrid.Items[lastIndex]);
                                }
                            }
                            finally
                            {
                                _isScrolling = false;
                            }
                        }), System.Windows.Threading.DispatcherPriority.Background);
                    }
                    catch
                    {
                        _isScrolling = false;
                        throw;
                    }
                }
                else
                {
                    _isScrolling = false;
                }
            }
        }

        private void LogView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.LogViewModel viewModel &&
                viewModel.LogEntries is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= FilteredLogEntries_CollectionChanged;
            }

            this.DataContextChanged -= LogView_DataContextChanged;
            this.Unloaded -= LogView_Unloaded;
        }
    }
}
