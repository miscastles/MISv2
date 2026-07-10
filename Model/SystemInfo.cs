using System;

namespace MIS.Model
{
    public class SystemInfo
    {
        public const int AllowedClockDifferenceMinutes = 2;

        // ==========================
        // Server Information
        // ==========================

        public DateTime ServerDateTime { get; set; }

        public DateTime ServerDate { get; set; }

        public TimeSpan ServerTime { get; set; }

        public string ServerTimeZone { get; set; }

        public string DatabaseVersion { get; set; }

        public DateTime BusinessDate { get; set; }

        public bool IsOnline { get; set; }

        // ==========================
        // Local PC Information
        // ==========================

        public DateTime LocalDateTime => DateTime.Now;

        public DateTime LocalDate => LocalDateTime.Date;

        public TimeSpan LocalTime => LocalDateTime.TimeOfDay;

        public string LocalTimeZone => TimeZoneInfo.Local.Id;

        // ==========================
        // Clock Comparison
        // ==========================

        public TimeSpan ClockDifference =>
            LocalDateTime - ServerDateTime;

        public double ClockDifferenceSeconds =>
            Math.Abs(ClockDifference.TotalSeconds);

        public double ClockDifferenceMinutes =>
            Math.Abs(ClockDifference.TotalMinutes);

        public bool IsClockSynchronized =>
            ClockDifferenceMinutes <= AllowedClockDifferenceMinutes;

        // ==========================
        // Time Zone Comparison
        // ==========================

        public bool IsTimeZoneSynchronized =>
            string.Equals(
                ServerTimeZone?.Trim(),
                LocalTimeZone,
                StringComparison.OrdinalIgnoreCase);
    }
}