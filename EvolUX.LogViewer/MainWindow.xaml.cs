using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace EvolUX.LogViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Register for the Loaded event to perform additional setup
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize UI state
            InitializeLogDetailsPanel();

            // Set up DataGrid selection changed event
            if (logEntriesGrid != null)
            {
                logEntriesGrid.SelectionChanged += LogEntriesGrid_SelectionChanged;
            }

            // Add a listener for the view model's property changes
            if (DataContext is INotifyPropertyChanged viewModel)
            {
                ((INotifyPropertyChanged)viewModel).PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // When the SelectedLogEntry changes, ensure UI updates correctly
            if (e.PropertyName == "SelectedLogEntry")
            {
                UpdateLogDetailsVisibility();
            }
        }

        private void InitializeLogDetailsPanel()
        {
            // Ensure expanders are initially collapsed
            if (DataContext is ViewModels.MainViewModel viewModel && viewModel.SelectedLogEntry == null)
            {
                if (variablesExpander != null) variablesExpander.Visibility = Visibility.Collapsed;
                if (exceptionDetailsExpander != null) exceptionDetailsExpander.Visibility = Visibility.Collapsed;
                if (exceptionDataExpander != null) exceptionDataExpander.Visibility = Visibility.Collapsed;
                if (innerExceptionsExpander != null) innerExceptionsExpander.Visibility = Visibility.Collapsed;
                if (stackTraceExpander != null) stackTraceExpander.Visibility = Visibility.Collapsed;
            }
        }

        private void LogEntriesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Force UI update after selection
            UpdateLogDetailsVisibility();
        }

        private void UpdateLogDetailsVisibility()
        {
            if (DataContext is ViewModels.MainViewModel viewModel)
            {
                var logEntry = viewModel.SelectedLogEntry;

                // Update panel visibility based on selected log entry
                if (logEntry != null)
                {
                    // Main details panel should be visible
                    if (logDetailsPanel != null)
                        logDetailsPanel.Visibility = Visibility.Visible;

                    // Variables expander
                    if (variablesExpander != null)
                        variablesExpander.Visibility = logEntry.HasVariables ? Visibility.Visible : Visibility.Collapsed;

                    // Exception details expander
                    if (exceptionDetailsExpander != null)
                        exceptionDetailsExpander.Visibility = logEntry.HasException ? Visibility.Visible : Visibility.Collapsed;

                    // Exception data expander
                    if (exceptionDataExpander != null)
                        exceptionDataExpander.Visibility = logEntry.HasExceptionData ? Visibility.Visible : Visibility.Collapsed;

                    // Inner exceptions expander
                    if (innerExceptionsExpander != null)
                        innerExceptionsExpander.Visibility = logEntry.HasInnerExceptions ? Visibility.Visible : Visibility.Collapsed;

                    // Stack trace expander
                    if (stackTraceExpander != null)
                        stackTraceExpander.Visibility = logEntry.HasStackTrace ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    // Hide log details panel when no log is selected
                    if (logDetailsPanel != null)
                        logDetailsPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void RefreshDetails_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel viewModel && viewModel.SelectedLogEntry != null)
            {
                // Force UI refresh - IMPORTANT: Only call RefreshProperties on LogEntry objects, not ExceptionInfo
                var logEntry = viewModel.SelectedLogEntry;
                logEntry.RefreshProperties(); // This is fine because it's directly on the LogEntry

                // Manually update UI visibility
                UpdateLogDetailsVisibility();

                // Manually ensure exception details are showing
                if (logEntry.HasException && exceptionDetailsExpander != null)
                {
                    exceptionDetailsExpander.Visibility = Visibility.Visible;
                    exceptionDetailsExpander.IsExpanded = true;
                }

                if (logEntry.HasStackTrace && stackTraceExpander != null)
                {
                    stackTraceExpander.Visibility = Visibility.Visible;
                    stackTraceExpander.IsExpanded = true;

                    // Ensure stack trace is displayed
                    if (stackTraceTextBox != null && logEntry.StackTrace != null)
                    {
                        stackTraceTextBox.Text = logEntry.StackTrace;
                    }
                }

                // Update exception message
                if (exceptionMessageTextBox != null && logEntry.ExceptionMessage != null)
                {
                    exceptionMessageTextBox.Text = logEntry.ExceptionMessage;
                }

                // Update exception type
                if (exceptionTypeTextBlock != null && logEntry.ExceptionType != null)
                {
                    exceptionTypeTextBlock.Text = logEntry.ExceptionType;
                }
            }
        }

        private void ShowExceptionDetails_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel viewModel && viewModel.SelectedLogEntry != null)
            {
                var logEntry = viewModel.SelectedLogEntry;

                // Build exception details
                var details = new System.Text.StringBuilder();

                if (!string.IsNullOrEmpty(logEntry.ExceptionType))
                {
                    details.AppendLine($"Exception Type: {logEntry.ExceptionType}");
                }

                if (!string.IsNullOrEmpty(logEntry.ExceptionMessage))
                {
                    details.AppendLine($"Message: {logEntry.ExceptionMessage}");
                }

                if (!string.IsNullOrEmpty(logEntry.StackTrace))
                {
                    details.AppendLine("\nStack Trace:");
                    details.AppendLine(logEntry.StackTrace);
                }

                if (logEntry.InnerExceptions != null && logEntry.InnerExceptions.Count > 0)
                {
                    details.AppendLine("\nInner Exceptions:");
                    int i = 1;
                    foreach (var inner in logEntry.InnerExceptions)
                    {
                        details.AppendLine($"[{i}] {inner.Type}: {inner.Message}");
                        if (!string.IsNullOrEmpty(inner.StackTrace))
                        {
                            details.AppendLine("Stack Trace:");
                            details.AppendLine(inner.StackTrace);
                        }
                        i++;
                    }
                }

                // Update the text box
                if (directExceptionTextBox != null)
                {
                    directExceptionTextBox.Text = details.ToString();
                }
            }
        }

        private void ShowSimpleLogDetails_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel viewModel && viewModel.SelectedLogEntry != null)
            {
                var logEntry = viewModel.SelectedLogEntry;
                var detailsWindow = new SimpleLogDetailsWindow();
                detailsWindow.DisplayLogDetails(logEntry);
                detailsWindow.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("No log entry selected", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void VariablesTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TreeView treeView && treeView.SelectedItem is Models.TreeNode selectedNode)
            {
                var detailsWindow = new VariableDetailsWindow
                {
                    Owner = this
                };
                detailsWindow.DisplayVariable(selectedNode);
                detailsWindow.ShowDialog();
                e.Handled = true;
            }
        }
    }
}