using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Tasks are stored in: tasks.json  next to the application exe.

namespace CyberAwareness
{
    public static class DatabaseHelper
    {
        // Path to the JSON file stored alongside the application
        private static readonly string FilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "tasks.json");

        private static int _nextId = 1;

        /// <summary>
        /// Initialises the task file. Creates it if it does not exist.
        /// Returns true on success and a status message.
        /// </summary>
        public static bool Initialise(out string message)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    File.WriteAllText(FilePath, "[]", Encoding.UTF8);
                }

                // Set next ID based on existing tasks
                string err;
                List<TaskItem> existing = GetAllTasks(out err);
                foreach (TaskItem t in existing)
                    if (t.Id >= _nextId) _nextId = t.Id + 1;

                message = "Tasks file ready: tasks.json";
                return true;
            }
            catch (Exception ex)
            {
                message = "File error: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Adds a new task. Returns the new ID or -1 on failure.
        /// </summary>
        public static int AddTask(string title, string description, string reminder, out string err)
        {
            err = string.Empty;
            try
            {
                string loadErr;
                List<TaskItem> tasks = GetAllTasks(out loadErr);

                TaskItem newTask = new TaskItem();
                newTask.Id = _nextId++;
                newTask.Title = title;
                newTask.Description = description ?? string.Empty;
                newTask.Reminder = reminder ?? string.Empty;
                newTask.IsComplete = false;
                newTask.CreatedAt = DateTime.Now;

                tasks.Add(newTask);
                SaveAll(tasks);
                return newTask.Id;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// Returns all tasks ordered newest first.
        /// </summary>
        public static List<TaskItem> GetAllTasks(out string err)
        {
            err = string.Empty;
            List<TaskItem> list = new List<TaskItem>();
            try
            {
                if (!File.Exists(FilePath))
                    return list;

                string json = File.ReadAllText(FilePath, Encoding.UTF8).Trim();
                if (json == "[]" || string.IsNullOrWhiteSpace(json))
                    return list;

                // Simple manual JSON parser for our known structure
                list = ParseTasks(json);
                list.Sort(delegate (TaskItem a, TaskItem b)
                {
                    return b.CreatedAt.CompareTo(a.CreatedAt);
                });
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
            return list;
        }

        /// <summary>
        /// Marks a task complete or incomplete.
        /// </summary>
        public static string SetComplete(int id, bool complete)
        {
            try
            {
                string loadErr;
                List<TaskItem> tasks = GetAllTasks(out loadErr);
                foreach (TaskItem t in tasks)
                {
                    if (t.Id == id)
                    {
                        t.IsComplete = complete;
                        break;
                    }
                }
                SaveAll(tasks);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Deletes a task by ID.
        /// </summary>
        public static string DeleteTask(int id)
        {
            try
            {
                string loadErr;
                List<TaskItem> tasks = GetAllTasks(out loadErr);
                TaskItem toRemove = null;
                foreach (TaskItem t in tasks)
                {
                    if (t.Id == id) { toRemove = t; break; }
                }
                if (toRemove != null) tasks.Remove(toRemove);
                SaveAll(tasks);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        
        // PRIVATE HELPERS
        

        // Saves the full task list back to the JSON file
        private static void SaveAll(List<TaskItem> tasks)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            for (int i = 0; i < tasks.Count; i++)
            {
                TaskItem t = tasks[i];
                sb.AppendLine("  {");
                sb.AppendLine("    \"Id\": " + t.Id + ",");
                sb.AppendLine("    \"Title\": \"" + Escape(t.Title) + "\",");
                sb.AppendLine("    \"Description\": \"" + Escape(t.Description) + "\",");
                sb.AppendLine("    \"Reminder\": \"" + Escape(t.Reminder) + "\",");
                sb.AppendLine("    \"IsComplete\": " + (t.IsComplete ? "true" : "false") + ",");
                sb.AppendLine("    \"CreatedAt\": \"" + t.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss") + "\"");
                sb.Append("  }");
                if (i < tasks.Count - 1) sb.AppendLine(",");
                else sb.AppendLine();
            }
            sb.AppendLine("]");
            File.WriteAllText(FilePath, sb.ToString(), Encoding.UTF8);
        }

        // Escapes special characters for JSON strings
        private static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "")
                    .Replace("\n", " ");
        }

        // Simple line-by-line JSON parser for our known task format
        private static List<TaskItem> ParseTasks(string json)
        {
            List<TaskItem> result = new List<TaskItem>();
            string[] lines = json.Split('\n');

            TaskItem current = null;
            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (line == "{")
                {
                    current = new TaskItem();
                    continue;
                }

                if ((line == "}" || line == "},") && current != null)
                {
                    result.Add(current);
                    current = null;
                    continue;
                }

                if (current == null) continue;

                // Parse each field
                if (line.StartsWith("\"Id\""))
                {
                    current.Id = ParseInt(line);
                }
                else if (line.StartsWith("\"Title\""))
                {
                    current.Title = ParseString(line);
                }
                else if (line.StartsWith("\"Description\""))
                {
                    current.Description = ParseString(line);
                }
                else if (line.StartsWith("\"Reminder\""))
                {
                    current.Reminder = ParseString(line);
                }
                else if (line.StartsWith("\"IsComplete\""))
                {
                    current.IsComplete = line.Contains("true");
                }
                else if (line.StartsWith("\"CreatedAt\""))
                {
                    string val = ParseString(line);
                    DateTime dt;
                    if (DateTime.TryParse(val, out dt))
                        current.CreatedAt = dt;
                }
            }
            return result;
        }

        // Extracts the integer value from a JSON line like: "Id": 3,
        private static int ParseInt(string line)
        {
            int colon = line.IndexOf(':');
            if (colon < 0) return 0;
            string val = line.Substring(colon + 1).Trim().TrimEnd(',');
            int result;
            int.TryParse(val, out result);
            return result;
        }

        // Extracts the string value from a JSON line like: "Title": "my task",
        private static string ParseString(string line)
        {
            int colon = line.IndexOf(':');
            if (colon < 0) return string.Empty;
            string val = line.Substring(colon + 1).Trim().TrimEnd(',');
            if (val.StartsWith("\"")) val = val.Substring(1);
            if (val.EndsWith("\"")) val = val.Substring(0, val.Length - 1);
            return val.Replace("\\\"", "\"").Replace("\\\\", "\\");
        }
    }

    /// <summary>Represents a single cybersecurity task.</summary>
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool IsComplete { get; set; }
        public DateTime CreatedAt { get; set; }

        public TaskItem()
        {
            Title = string.Empty;
            Description = string.Empty;
            Reminder = string.Empty;
        }
    }
}
