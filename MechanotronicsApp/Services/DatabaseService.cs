using System;
using System.Data.SQLite;
using System.IO;
using MechanotronicsApp.Models;
using MechanotronicsApp.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MechanotronicsApp.Services
{
    public class DatabaseService : IDatabaseService, IDisposable
    {
        private readonly string _dbPath = "data.db";
        private readonly ILoggingService _loggingService;
        private readonly SQLiteConnection _connection;
        private bool _disposed;

        public DatabaseService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }
            _connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
            _connection.Open();
            InitializeDatabase();
        }

        ~DatabaseService()
        {
            Dispose(false);
        }

        private void InitializeDatabase()
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                var command = _connection.CreateCommand();
                command.Transaction = transaction;
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
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _loggingService.LogError("Error initializing database", ex);
                throw;
            }
        }

        public void SaveCar(DateTime timestamp, string carName)
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                var command = _connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT OR REPLACE INTO Cars (Timestamp, CarName)
                    VALUES (@Timestamp, @CarName)";

                command.Parameters.AddWithValue("@Timestamp", timestamp.ToString("O"));
                command.Parameters.AddWithValue("@CarName", carName);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _loggingService.LogError("Error saving car to database", ex);
                throw;
            }
        }

        public void SaveDriver(DateTime timestamp, string driverName)
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                var command = _connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT OR REPLACE INTO Drivers (Timestamp, DriverName)
                    VALUES (@Timestamp, @DriverName)";

                command.Parameters.AddWithValue("@Timestamp", timestamp.ToString("O"));
                command.Parameters.AddWithValue("@DriverName", driverName);

                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _loggingService.LogError("Error saving driver to database", ex);
                throw;
            }
        }

        public List<DataItem> GetAllData()
        {
            using var transaction = _connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                var command = _connection.CreateCommand();
                command.Transaction = transaction;
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
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _loggingService.LogError("Error loading data from database", ex);
                throw;
            }
        }

        public void Clear()
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                var command = _connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM Cars; DELETE FROM Drivers;";
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _loggingService.LogError("Error clearing database", ex);
                throw;
            }
        }

        public async Task AddDataItemAsync(DataItem item)
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                if (item.CarName != null)
                {
                    var carCommand = _connection.CreateCommand();
                    carCommand.Transaction = transaction;
                    carCommand.CommandText = @"
                        INSERT OR REPLACE INTO Cars (Timestamp, CarName)
                        VALUES (@Timestamp, @CarName)";

                    carCommand.Parameters.AddWithValue("@Timestamp", item.Timestamp.ToString("O"));
                    carCommand.Parameters.AddWithValue("@CarName", item.CarName);
                    carCommand.ExecuteNonQuery();
                }

                if (item.DriverName != null)
                {
                    var driverCommand = _connection.CreateCommand();
                    driverCommand.Transaction = transaction;
                    driverCommand.CommandText = @"
                        INSERT OR REPLACE INTO Drivers (Timestamp, DriverName)
                        VALUES (@Timestamp, @DriverName)";

                    driverCommand.Parameters.AddWithValue("@Timestamp", item.Timestamp.ToString("O"));
                    driverCommand.Parameters.AddWithValue("@DriverName", item.DriverName);
                    driverCommand.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _loggingService.LogError("Error adding data item to database", ex);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _connection?.Close();
                _connection?.Dispose();
            }

            _disposed = true;
        }
    }
} 