using System;
using System.Windows;

namespace EvolUX.LogViewer
{
    public partial class SimpleLogDetailsWindow : Window
    {
        public SimpleLogDetailsWindow()
        {
            InitializeComponent();
        }

        public void DisplayLogDetails(Models.LogEntry log)
        {
            if (log == null)
            {
                System.Windows.MessageBox.Show("No log entry provided", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Set simple properties
            timestampText.Text = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
            levelText.Text = log.Level;
            traceIdText.Text = log.TraceId;
            messageText.Text = log.Message;

            // Set exception properties if available
            exceptionTypeText.Text = log.ExceptionType ?? "(none)";
            exceptionMessageText.Text = log.ExceptionMessage ?? "(none)";
            stackTraceText.Text = log.StackTrace ?? "(none)";

            // Set title to include message
            this.Title = $"Log Details - {log.Timestamp:yyyy-MM-dd HH:mm:ss} - {log.Level}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}