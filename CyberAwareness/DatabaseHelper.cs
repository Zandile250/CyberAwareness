using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

// Handles all MySQL database operations for the Task Assistant .
// The application automatically creates the database and table on first run.

namespace CyberAwareness
{
    public static class DatabaseHelper
    {
        // Connection settings
        private const string Server = "localhost";
        private const string Database = "cyberbot_db";
        private const string User = "root";
        private const string Password = "";          
        private const string Port = "3306";

        private static string ConnectionString
        {
            get
            {
                return string.Format(
                    "Server={0};Port={1};Database={2};Uid={3};Pwd={4};CharSet=utf8mb4;",
                    Server, Port, Database, User, Password);
            }
        }

        
        public static bool Initialise(out string message)
        {
            try
            {
                // Connect without specifying a database so we can CREATE DATABASE
                string rootConn = string.Format(
                    "Server={0};Port={1};Uid={2};Pwd={3};CharSet=utf8mb4;",
                    Server, Port, User, Password);

                using (MySqlConnection root = new MySqlConnection(rootConn))
                {
                    root.Open();
                    string createDb = string.Format(
                        "CREATE DATABASE IF NOT EXISTS `{0}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;",
                        Database);
                    new MySqlCommand(createDb, root).ExecuteNonQuery();
                }

                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    string createTable =
                        "CREATE TABLE IF NOT EXISTS tasks (" +
                        "  id          INT AUTO_INCREMENT PRIMARY KEY," +
                        "  title       VARCHAR(200) NOT NULL," +
                        "  description TEXT," +
                        "  reminder    VARCHAR(200)," +
                        "  is_complete TINYINT(1) DEFAULT 0," +
                        "  created_at  DATETIME DEFAULT CURRENT_TIMESTAMP" +
                        ") ENGINE=InnoDB;";
                    new MySqlCommand(createTable, conn).ExecuteNonQuery();
                }

                message = "Connected to MySQL - cyberbot_db";
                return true;
            }
            catch (Exception ex)
            {
                message = "DB Error: " + ex.Message;
                return false;
            }
        }

      
        /// Inserts a new task. Returns the new ID or -1 on failure.
       
        public static int AddTask(string title, string description, string reminder, out string err)
        {
            err = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    string sql =
                        "INSERT INTO tasks (title, description, reminder) " +
                        "VALUES (@title, @desc, @reminder); " +
                        "SELECT LAST_INSERT_ID();";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description ?? string.Empty);
                        cmd.Parameters.AddWithValue("@reminder", reminder ?? string.Empty);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return -1;
            }
        }

        
        /// Returns all tasks ordered newest first.
        
        public static List<TaskItem> GetAllTasks(out string err)
        {
            err = string.Empty;
            List<TaskItem> list = new List<TaskItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    string sql =
                        "SELECT id, title, description, reminder, is_complete, created_at " +
                        "FROM tasks ORDER BY created_at DESC;";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TaskItem
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString("description"),
                                Reminder = reader.IsDBNull(reader.GetOrdinal("reminder")) ? string.Empty : reader.GetString("reminder"),
                                IsComplete = reader.GetInt32("is_complete") == 1,
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
            return list;
        }

        
        /// Marks a task complete or incomplete. Returns empty string on success.
        
        public static string SetComplete(int id, bool complete)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "UPDATE tasks SET is_complete = @c WHERE id = @id;", conn))
                    {
                        cmd.Parameters.AddWithValue("@c", complete ? 1 : 0);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        
        /// Deletes a task. Returns empty string on success.
        
        public static string DeleteTask(int id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tasks WHERE id = @id;", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    /// <summary>Represents a single cybersecurity task row from the database.</summary>
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
