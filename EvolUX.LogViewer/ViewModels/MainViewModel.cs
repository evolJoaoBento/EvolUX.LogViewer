using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EvolUX.LogViewer.Models;
using EvolUX.LogViewer.Services;
using Microsoft.Win32;

namespace EvolUX.LogViewer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ILogParserService _logParserService;
        private readonly ILogSearchService _logSearchService;

        public ObservableCollection<LogEntry> LogEntries { get; } = new ObservableCollection<LogEntry>();
        public ObservableCollection<string> LogLevels { get; } = new ObservableCollection<string>();

        private string _selectedLogLevel = "All";
        public string SelectedLogLevel
        {
            get => _selectedLogLevel;
            set
            {
                if (_selectedLogLevel != value)
                {
                    _selectedLogLevel = value;
                    OnPropertyChanged();
                    FilterLogs();
                }
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _fromDate = DateTime.Today.AddDays(-7);
        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged();
                    FilterLogs();
                }
            }
        }

        private DateTime _toDate = DateTime.Today.AddDays(1);
        public DateTime ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged();
                    FilterLogs();
                }
            }
        }

        private LogEntry? _selectedLogEntry;
        public LogEntry? SelectedLogEntry
        {
            get => _selectedLogEntry;
            set
            {
                if (_selectedLogEntry != value)
                {
                    _selectedLogEntry = value;
                    OnPropertyChanged();

                    // .NET 8 addition - generate tree nodes for variables and exception data
                    if (_selectedLogEntry != null)
                    {
                        GenerateTreeNodes(_selectedLogEntry);
                    }
                }
            }
        }

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        // New properties for tree nodes 
        public ObservableCollection<Models.TreeNode> VariablesTreeNodes { get; } = new ObservableCollection<Models.TreeNode>();
        public ObservableCollection<Models.TreeNode> ExceptionDataTreeNodes { get; } = new ObservableCollection<Models.TreeNode>();

        // Commands
        public ICommand OpenLogFolderCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ExportLogsCommand { get; }

        public MainViewModel(ILogParserService logParserService, ILogSearchService logSearchService)
        {
            _logParserService = logParserService;
            _logSearchService = logSearchService;

            // Initialize commands
            OpenLogFolderCommand = new RelayCommand(OpenLogFolder);
            SearchCommand = new RelayCommand(SearchLogs);
            ExportLogsCommand = new RelayCommand(ExportLogs, CanExportLogs);

            // Initialize log levels
            LogLevels.Add("All");
            LogLevels.Add("DEBUG");
            LogLevels.Add("INFO");
            LogLevels.Add("WARNING");
            LogLevels.Add("ERROR");
        }

        private void OpenLogFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select Logs Folder",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var folderPath = dialog.SelectedPath;
                LoadLogsFromFolder(folderPath);
            }
        }

        private async void LoadLogsFromFolder(string folderPath)
        {
            try
            {
                StatusMessage = "Loading logs...";

                var logs = await Task.Run(() => _logParserService.ParseLogsFromFolder(folderPath));

                LogEntries.Clear();
                foreach (var log in logs)
                {
                    LogEntries.Add(log);
                }

                StatusMessage = $"Loaded {LogEntries.Count} log entries from {folderPath}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading logs: {ex.Message}";
            }
        }

        private void FilterLogs()
        {
            if (LogEntries.Count == 0) return;

            var filteredLogs = _logSearchService.FilterLogsByDateAndLevel(
                LogEntries, FromDate, ToDate, SelectedLogLevel);

            LogEntries.Clear();
            foreach (var log in filteredLogs)
            {
                LogEntries.Add(log);
            }

            StatusMessage = $"Filtered to {LogEntries.Count} log entries";
        }

        private void SearchLogs()
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return;

            var searchResults = _logSearchService.SearchLogs(LogEntries, SearchText);

            LogEntries.Clear();
            foreach (var log in searchResults)
            {
                LogEntries.Add(log);
            }

            StatusMessage = $"Found {LogEntries.Count} matching log entries";
        }

        private void ExportLogs()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".txt",
                Title = "Export Logs"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    StatusMessage = "Exporting logs...";

                    if (dialog.FileName.EndsWith(".json"))
                    {
                        _logParserService.ExportLogsToJson(LogEntries, dialog.FileName);
                    }
                    else
                    {
                        _logParserService.ExportLogsToText(LogEntries, dialog.FileName);
                    }

                    StatusMessage = $"Exported {LogEntries.Count} log entries to {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error exporting logs: {ex.Message}";
                }
            }
        }

        private bool CanExportLogs()
        {
            return LogEntries.Count > 0;
        }

        // New helper method for .NET 8 
        private void GenerateTreeNodes(LogEntry logEntry)
        {
            VariablesTreeNodes.Clear();
            ExceptionDataTreeNodes.Clear();

            // Generate variables tree nodes
            if (logEntry.Variables != null && logEntry.Variables.Count > 0)
            {
                foreach (var variable in logEntry.Variables)
                {
                    var node = CreateTreeNode(variable.Key, variable.Value);
                    VariablesTreeNodes.Add(node);
                }
            }

            // Generate exception data tree nodes
            if (logEntry.ExceptionData != null && logEntry.ExceptionData.Count > 0)
            {
                foreach (var data in logEntry.ExceptionData)
                {
                    var node = CreateTreeNode(data.Key, data.Value);
                    ExceptionDataTreeNodes.Add(node);
                }
            }
        }

        private Models.TreeNode CreateTreeNode(string key, object? value)
        {
            if (value == null)
            {
                return new Models.TreeNode { Key = key, Value = "null" };
            }

            // For simple types, just return the value
            if (value is string or int or double or bool or DateTime)
            {
                return new Models.TreeNode { Key = key, Value = value.ToString() };
            }

            // For dictionaries, create child nodes
            if (value is Dictionary<string, object> dict)
            {
                var node = new Models.TreeNode { Key = key };
                foreach (var item in dict)
                {
                    node.Children.Add(CreateTreeNode(item.Key, item.Value));
                }
                return node;
            }

            // For lists or arrays, create child nodes
            if (value is System.Collections.IEnumerable items && !(value is string))
            {
                var node = new Models.TreeNode { Key = key };
                int index = 0;
                foreach (var item in items)
                {
                    node.Children.Add(CreateTreeNode($"[{index}]", item));
                    index++;
                }
                return node;
            }

            // For complex objects, try to extract properties
            var type = value.GetType();
            if (type.IsClass && type != typeof(string))
            {
                var node = new Models.TreeNode { Key = key };
                var properties = type.GetProperties();
                if (properties.Length > 0)
                {
                    foreach (var prop in properties)
                    {
                        try
                        {
                            var propValue = prop.GetValue(value);
                            node.Children.Add(CreateTreeNode(prop.Name, propValue));
                        }
                        catch
                        {
                            node.Children.Add(new Models.TreeNode { Key = prop.Name, Value = "<Error reading value>" });
                        }
                    }
                    return node;
                }
            }

            // Fallback
            return new Models.TreeNode { Key = key, Value = value.ToString() };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple relay command implementation using source generators in .NET 8
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
