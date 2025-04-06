using System;
using System.Data.SQLite;
using System.IO;
using MechanotronicsApp.Models;
using MechanotronicsApp.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MechanotronicsApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _dbPath = "data.db";
        private readonly ILoggingService _loggingService;

        public DatabaseService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            using var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Cars (
                    Timestamp TEXT PRIMARY KEY,
                    CarName TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Drivers (
                    Timestamp TEXT PRIMARY KEY,
                    DriverName TEXT NOT NULL
                )";
            command.ExecuteNonQuery();
        }

        public void SaveCar(DateTime timestamp, string carName)
        {
            try
            {
                using var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT OR REPLACE INTO Cars (Timestamp, CarName)
                    VALUES (@Timestamp, @CarName)";

                command.Parameters.AddWithValue("@Timestamp", timestamp.ToString("O"));
                command.Parameters.AddWithValue("@CarName", carName);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error saving car to database", ex);
                throw;
            }
        }

        public void SaveDriver(DateTime timestamp, string driverName)
        {
            try
            {
                using var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT OR REPLACE INTO Drivers (Timestamp, DriverName)
                    VALUES (@Timestamp, @DriverName)";

                command.Parameters.AddWithValue("@Timestamp", timestamp.ToString("O"));
                command.Parameters.AddWithValue("@DriverName", driverName);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error saving driver to database", ex);
                throw;
            }
        }

        public List<DataItem> GetAllData()
        {
            try
            {
                using var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 
                        COALESCE(c.Timestamp, d.Timestamp) as Timestamp,
                        c.CarName,
                        d.DriverName
                    FROM 
                        (SELECT Timestamp, CarName FROM Cars) c
                    FULL OUTER JOIN 
                        (SELECT Timestamp, DriverName FROM Drivers) d
                    ON c.Timestamp = d.Timestamp
                    ORDER BY Timestamp DESC";

                var result = new List<DataItem>();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new DataItem
                    {
                        Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                        CarName = reader["CarName"] as string,
                        DriverName = reader["DriverName"] as string
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error loading data from database", ex);
                throw;
            }
        }

        public void Clear()
        {
            try
            {
                using var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Cars; DELETE FROM Drivers;";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error clearing database", ex);
                throw;
            }
        }

        public async Task AddDataItemAsync(DataItem item)
        {
            try
            {
                if (item.CarName != null)
                {
                    SaveCar(item.Timestamp, item.CarName);
                }
                if (item.DriverName != null)
                {   
                    SaveDriver(item.Timestamp, item.DriverName);
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error adding data item to database", ex);
                throw;
            }
        }
    }
} 