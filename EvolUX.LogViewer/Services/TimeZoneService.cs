using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EvolUX.LogViewer.Services
{
    public interface ITimeZoneService
    {
        ObservableCollection<TimeZoneInfo> AvailableTimeZones { get; }
        TimeZoneInfo SelectedTimeZone { get; set; }
        DateTime ConvertFromUtc(DateTime utcTime);
        string GetTimeZoneDisplayName();
        event EventHandler TimeZoneChanged;
    }

    public class TimeZoneService : ITimeZoneService
    {
        public ObservableCollection<TimeZoneInfo> AvailableTimeZones { get; } = new ObservableCollection<TimeZoneInfo>();

        private TimeZoneInfo _selectedTimeZone;
        public TimeZoneInfo SelectedTimeZone
        {
            get => _selectedTimeZone;
            set
            {
                if (_selectedTimeZone != value)
                {
                    _selectedTimeZone = value;
                    TimeZoneChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler TimeZoneChanged;

        public TimeZoneService()
        {
            InitializeTimeZones();
        }

        private void InitializeTimeZones()
        {
            // Add UTC first
            var utcTimeZone = TimeZoneInfo.Utc;
            AvailableTimeZones.Add(utcTimeZone);

            // Add all other time zones, ordered by offset
            foreach (var timeZone in TimeZoneInfo.GetSystemTimeZones()
                .Where(tz => tz.Id != "UTC")
                .OrderBy(tz => tz.BaseUtcOffset))
            {
                AvailableTimeZones.Add(timeZone);
            }

            // Set local time zone as default
            _selectedTimeZone = TimeZoneInfo.Local;
        }

        public DateTime ConvertFromUtc(DateTime utcTime)
        {
            if (utcTime.Kind != DateTimeKind.Utc)
            {
                // If not explicitly UTC, ensure we treat it as UTC for conversion
                utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            }

            if (_selectedTimeZone == null)
                return utcTime;

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, _selectedTimeZone);
        }

        public string GetTimeZoneDisplayName()
        {
            return _selectedTimeZone?.DisplayName ?? "UTC";
        }
    }
}