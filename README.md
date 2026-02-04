# EvolUX Log Viewer

A modern, feature-rich log viewer application designed for software developers and system administrators to efficiently analyze and debug application logs.

![image](https://github.com/user-attachments/assets/30bf6d35-2a24-4811-b531-d4218934c817)


## Features

- **Intuitive Interface**: Clean, modern UI with a split-pane view for easy navigation
- **Advanced Filtering**: Filter logs by date range, log level, and free-text search
- **Exception Details**: Comprehensive view of exceptions, including stack traces and inner exceptions
- **Time Zone Support**: View logs in your preferred time zone
- **Tree View Variables**: Hierarchical view of log variables and exception data
- **Export Capability**: Export logs to text or JSON formats
- **Multiple Display Options**: Grid view for overview, detailed view for in-depth analysis

## Getting Started

### Prerequisites

- Windows 10 or later
- .NET 8.0 Runtime or SDK

### Installation

1. Download the latest release from the [Releases](https://github.com/yourusername/EvolUX.LogViewer/releases) page
2. Extract the ZIP file to a location of your choice
3. Run `EvolUX.LogViewer.exe`

Alternatively, build from source:

```
git clone https://github.com/yourusername/EvolUX.LogViewer.git
cd EvolUX.LogViewer
dotnet build -c Release
```

## Usage Guide

### Opening Log Files

1. Click the "Open Log Folder" button in the top-left corner
2. Navigate to the folder containing your log files
3. Select the folder and click "OK"

The application will scan for log files and display them in the left panel.

### Filtering Logs

- **Date Range**: Use the "From" and "To" date pickers to specify a date range
- **Log Level**: Select a specific log level from the dropdown menu (DEBUG, INFO, WARNING, ERROR, or All)
- **Search**: Enter text in the search box and click "Search" to find logs containing specific text

### Viewing Log Details

1. Click on any log entry in the left panel to view its details
2. The right panel will display:
   - Basic log information (timestamp, level, trace ID, message)
   - Exception details (if present)
   - Variables (if available)
   - Stack trace (for exceptions)
   - Inner exceptions (if available)

### Time Zone Selection

1. Use the "Time Zone" dropdown in the top-right corner to select your preferred time zone
2. All timestamps will automatically convert to the selected time zone

### Exporting Logs

1. Load the logs you want to export
2. Apply any filters if needed
3. Use keyboard shortcut Ctrl+E or find the Export option in the menu
4. Choose between Text or JSON format
5. Select a location to save the exported file

## Log Format Support

EvolUX Log Viewer currently supports the following log formats:

- JSON Line Format (`.jsonl` files)
- Structured logs with the following fields:
  - `Timestamp`: Date and time of the log entry
  - `Level`: Log level (DEBUG, INFO, WARNING, ERROR)
  - `TraceId`: Correlation/trace identifier
  - `Message`: The log message
  - `Variables`: Key-value pairs of contextual data (optional)
  - `ExceptionType`: Type of exception (optional)
  - `ExceptionMessage`: Exception message (optional)
  - `StackTrace`: Exception stack trace (optional)
  - `InnerExceptions`: List of inner exceptions (optional)
  - `ExceptionData`: Additional exception data (optional)

## Advanced Features

### Exception Analysis

For logs containing exceptions, EvolUX Log Viewer provides:

- Syntax-highlighted stack traces
- Expandable/collapsible sections for inner exceptions
- Direct links to source code when file paths are present in stack traces
- Ability to copy any part of the exception details for sharing

### Variable Inspection

For logs with variable data:

- Tree view for hierarchical exploration of complex objects
- Expandable nodes for nested properties
- Search capability within variable values

## Troubleshooting

### Log Files Not Loading

- Ensure logs are in the supported format (JSONL)
- Check file permissions
- Look for errors in the status bar at the bottom of the application

### Application Performance

For large log files:

- Use date filtering to limit the number of logs loaded
- Close other resource-intensive applications
- Consider exporting filtered logs to a new file for faster loading

## Building from Source

1. Clone the repository
2. Open the solution in Visual Studio 2022 or later
3. Restore NuGet packages
4. Build the solution

Required NuGet packages:
- Microsoft.Extensions.DependencyInjection
- Newtonsoft.Json
- System.Linq.Dynamic.Core

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with WPF and .NET 8
- Icons from [Material Design Icons](https://materialdesignicons.com/)
- Uses Newtonsoft.Json for JSON parsing
