using MechanotronicsApp.Interfaces;
using System;
using System.IO;
using System.Threading;

namespace MechanotronicsApp.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly string _logDirectory;
        private string _currentLogFile;
        private readonly Timer _logFileTimer;
        private readonly object _lock = new object();

        public LoggingService()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(_logDirectory);
            CreateNewLogFile();
            _logFileTimer = new Timer(CreateNewLogFile, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        private void CreateNewLogFile(object state = null)
        {
            lock (_lock)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                _currentLogFile = Path.Combine(_logDirectory, $"log_{timestamp}.txt");
                File.WriteAllText(_currentLogFile, $"Log file created at {DateTime.Now}\n");
            }
        }

        public void LogInformation(string message)
        {
            Log("INFO", message);
        }

        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        public void LogError(string message, Exception ex = null)
        {
            var errorMessage = ex != null ? $"{message}\nException: {ex}" : message;
            Log("ERROR", errorMessage);
        }
     
        private void Log(string level, string message)
        {
            lock (_lock)
            {
                var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}\n";
                File.AppendAllText(_currentLogFile, logMessage);
            }
        }
      
    }
} 