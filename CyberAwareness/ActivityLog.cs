using System;
using System.Collections.Generic;
using System.Text;

// Records every action the chatbot takes so they can be reviewed in the Activity Log tab.
// This is an in-memory log; entries are lost when the app closes (by design for privacy).

namespace CyberAwareness
{
    // A single log entry with a timestamp and description
    public class LogEntry
    {
        public DateTime Time { get; set; }
        public string Entry { get; set; }

        public LogEntry(DateTime time, string entry)
        {
            Time = time;
            Entry = entry;
        }
    }

    public static class ActivityLog
    {
        private static readonly List<LogEntry> _entries = new List<LogEntry>();

        /// <summary>Adds a timestamped entry to the log.</summary>
        public static void Add(string action)
        {
            _entries.Add(new LogEntry(DateTime.Now, action));
        }

        /// <summary>Returns all log entries for display.</summary>
        public static IReadOnlyList<LogEntry> Entries
        {
            get { return _entries.AsReadOnly(); }
        }

        /// <summary>Clears all entries.</summary>
        public static void Clear()
        {
            _entries.Clear();
        }

        /// <summary>Exports the log as plain text.</summary>
        public static string ToText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogEntry e in _entries)
                sb.AppendLine("[" + e.Time.ToString("HH:mm:ss") + "]  " + e.Entry);
            return sb.ToString();
        }
    }
}

