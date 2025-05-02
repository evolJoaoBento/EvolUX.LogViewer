using EvolUX.LogViewer.Models;
using System.Linq.Dynamic.Core;

namespace EvolUX.LogViewer.Services
{
    public interface ILogSearchService
    {
        IEnumerable<LogEntry> FilterLogsByDateAndLevel(
            IEnumerable<LogEntry> logs,
            DateTime fromDate,
            DateTime toDate,
            string logLevel);

        IEnumerable<LogEntry> SearchLogs(
            IEnumerable<LogEntry> logs,
            string searchText);

        IEnumerable<LogEntry> SearchLogsByTraceId(
            IEnumerable<LogEntry> logs,
            string traceId);
    }
    public class LogSearchService : ILogSearchService
    {
        public IEnumerable<LogEntry> FilterLogsByDateAndLevel(
            IEnumerable<LogEntry> logs,
            DateTime fromDate,
            DateTime toDate,
            string logLevel)
        {
            // Adjust toDate to include the entire day
            var adjustedToDate = toDate.AddDays(1).AddSeconds(-1);

            var query = logs.Where(l => l.Timestamp >= fromDate &&
                                       l.Timestamp <= adjustedToDate);

            if (logLevel != "All")
            {
                query = query.Where(l => l.Level == logLevel);
            }

            return query;
        }

        public IEnumerable<LogEntry> SearchLogs(
            IEnumerable<LogEntry> logs,
            string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return logs;

            // Convert to lowercase for case-insensitive search
            var search = searchText.ToLower();

            // First check if this might be a trace ID
            if (searchText.Length > 20 && !searchText.Contains(' '))
            {
                var traceResults = SearchLogsByTraceId(logs, searchText);
                if (traceResults.Any())
                    return traceResults;
            }

            return logs.Where(l =>
                (l.Message?.ToLower().Contains(search) ?? false) ||
                (l.Level?.ToLower().Contains(search) ?? false) ||
                (l.TraceId?.ToLower().Contains(search) ?? false) ||
                (l.ExceptionType?.ToLower().Contains(search) ?? false) ||
                (l.ExceptionMessage?.ToLower().Contains(search) ?? false) ||
                (l.StackTrace?.ToLower().Contains(search) ?? false) ||
                (l.Variables != null && JsonContainsText(l.Variables, search)) ||
                (l.ExceptionData != null && JsonContainsText(l.ExceptionData, search)) ||
                (l.InnerExceptions != null && l.InnerExceptions.Any(e =>
                    (e.Type?.ToLower().Contains(search) ?? false) ||
                    (e.Message?.ToLower().Contains(search) ?? false) ||
                    (e.StackTrace?.ToLower().Contains(search) ?? false)))
            );
        }

        public IEnumerable<LogEntry> SearchLogsByTraceId(
            IEnumerable<LogEntry> logs,
            string traceId)
        {
            if (string.IsNullOrWhiteSpace(traceId))
                return Enumerable.Empty<LogEntry>();

            return logs.Where(l => l.TraceId == traceId);
        }

        private bool JsonContainsText(Dictionary<string, object> dict, string searchText)
        {
            if (dict == null) return false;

            // Check keys
            if (dict.Keys.Any(k => k.ToLower().Contains(searchText)))
                return true;

            // Check values
            foreach (var value in dict.Values)
            {
                if (value == null) continue;

                if (value is string strValue && strValue.ToLower().Contains(searchText))
                    return true;

                // For other types, convert to string and check
                var valueString = value.ToString();
                if (valueString != null && valueString.ToLower().Contains(searchText))
                    return true;
            }

            return false;
        }
    }
}