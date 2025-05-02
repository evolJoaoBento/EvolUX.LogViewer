using System.IO;
using System.Text;
using System.Text.Json;
using EvolUX.LogViewer.Models;
using Newtonsoft.Json;

namespace EvolUX.LogViewer.Services
{
    public interface ILogParserService
    {
        IEnumerable<LogEntry> ParseLogsFromFolder(string folderPath);
        void ExportLogsToText(IEnumerable<LogEntry> logs, string filePath);
        void ExportLogsToJson(IEnumerable<LogEntry> logs, string filePath);
    }
    public class LogParserService : ILogParserService
    {
        public IEnumerable<LogEntry> ParseLogsFromFolder(string folderPath)
        {
            var logs = new List<LogEntry>();

            // Find all JSON and text log files
            var jsonFiles = Directory.GetFiles(folderPath, "logs.jsonl", SearchOption.AllDirectories);

            // Parse JSON log files
            foreach (var file in jsonFiles)
            {
                try
                {
                    var lines = File.ReadAllLines(file);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        try
                        {
                            var logEntry = JsonConvert.DeserializeObject<LogEntry>(line);
                            if (logEntry != null)
                            {
                                logs.Add(logEntry);
                            }
                        }
                        catch (Newtonsoft.Json.JsonException)
                        {
                            // Skip invalid JSON
                        }
                    }
                }
                catch (Exception)
                {
                    // Skip files with errors
                }
            }

            // Sort by timestamp
            return logs.OrderByDescending(l => l.Timestamp);
        }

        public void ExportLogsToText(IEnumerable<LogEntry> logs, string filePath)
        {
            var sb = new StringBuilder();

            foreach (var log in logs)
            {
                sb.AppendLine($"[{log.Timestamp:yyyy-MM-ddTHH:mm:ss.fff}Z] [{log.Level}] [Trace: {log.TraceId}]");
                sb.AppendLine($"Message: {log.Message}");

                if (log.Variables != null && log.Variables.Count > 0)
                {
                    sb.AppendLine("Variables:");
                    foreach (var variable in log.Variables)
                    {
                        sb.AppendLine($"  {variable.Key}: {JsonConvert.SerializeObject(variable.Value)}");
                    }
                }

                if (!string.IsNullOrEmpty(log.ExceptionType))
                {
                    sb.AppendLine($"Exception: {log.ExceptionType}");
                    sb.AppendLine($"Message: {log.ExceptionMessage}");

                    if (log.ExceptionData != null && log.ExceptionData.Count > 0)
                    {
                        sb.AppendLine("Exception Data:");
                        foreach (var data in log.ExceptionData)
                        {
                            sb.AppendLine($"  {data.Key}: {JsonConvert.SerializeObject(data.Value)}");
                        }
                    }

                    if (log.InnerExceptions != null && log.InnerExceptions.Count > 0)
                    {
                        sb.AppendLine("Inner Exceptions:");
                        for (int i = 0; i < log.InnerExceptions.Count; i++)
                        {
                            var inner = log.InnerExceptions[i];
                            sb.AppendLine($"  [{i + 1}] {inner.Type}: {inner.Message}");
                            if (!string.IsNullOrEmpty(inner.StackTrace))
                            {
                                sb.AppendLine($"  Stack Trace:");
                                sb.AppendLine(inner.StackTrace);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(log.StackTrace))
                    {
                        sb.AppendLine("Stack Trace:");
                        sb.AppendLine(log.StackTrace);
                    }
                }

                sb.AppendLine(new string('-', 80));
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportLogsToJson(IEnumerable<LogEntry> logs, string filePath)
        {
            var logsArray = logs.ToArray();
            var json = JsonConvert.SerializeObject(logsArray, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}