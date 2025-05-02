namespace EvolUX.LogViewer.Models
{
    public class LogEntry
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
    }

    public class ExceptionInfo
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }

        // UI Helper Property
        public bool HasStackTrace => !string.IsNullOrEmpty(StackTrace);
    }

    public class TreeNode
    {
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public bool HasValue => !string.IsNullOrEmpty(Value);
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
