using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EvolUX.LogViewer.Models
{
    public class LogEntry : INotifyPropertyChanged
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object>? Variables { get; set; }
        public string? ExceptionType { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public List<ExceptionInfo>? InnerExceptions { get; set; }
        public Dictionary<string, object>? ExceptionData { get; set; }

        // UI Helper Properties
        public bool HasVariables => Variables != null && Variables.Count > 0;
        public bool HasException => !string.IsNullOrEmpty(ExceptionType);
        public bool HasExceptionData => ExceptionData != null && ExceptionData.Count > 0;
        public bool HasInnerExceptions => InnerExceptions != null && InnerExceptions.Count > 0;
        public bool HasStackTrace => !string.IsNullOrEmpty(StackTrace);

        // Method to force UI refresh
        public void RefreshProperties()
        {
            // Notify UI that all "Has*" properties might have changed
            OnPropertyChanged(nameof(HasVariables));
            OnPropertyChanged(nameof(HasException));
            OnPropertyChanged(nameof(HasExceptionData));
            OnPropertyChanged(nameof(HasInnerExceptions));
            OnPropertyChanged(nameof(HasStackTrace));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Note: ExceptionInfo is kept as a separate class without implementing INotifyPropertyChanged
    // since it's only used within LogEntry and doesn't need to directly notify the UI
    public class ExceptionInfo
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }

        // UI Helper Property - this is just a computed property, not a notifiable one
        public bool HasStackTrace => !string.IsNullOrEmpty(StackTrace);
    }

    public class TreeNode
    {
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public bool HasValue => !string.IsNullOrEmpty(Value);
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

        // Store the original raw value for popup display
        public object? RawValue { get; set; }
    }
}