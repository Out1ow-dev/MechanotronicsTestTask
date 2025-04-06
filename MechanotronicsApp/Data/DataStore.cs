using MechanotronicsApp.Models;
using MechanotronicsApp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MechanotronicsApp.Data
{
    public class DataStore : IDataStore
    {
        private readonly ConcurrentDictionary<DateTime, DataItem> _data = new();
        private readonly IDatabaseService _databaseService;
        private readonly ILoggingService _loggingService;

        public DataStore(IDatabaseService databaseService, ILoggingService loggingService)
        {
            _databaseService = databaseService;
            _loggingService = loggingService;
        }

        public void AddCar(DateTime timestamp, string carName)
        {
            try
            {
                var item = _data.GetOrAdd(timestamp, new DataItem { Timestamp = timestamp });
                item.CarName = carName;
                _databaseService.SaveCar(timestamp, carName);
                _loggingService.LogInformation($"Car {carName} added at {timestamp}");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error adding car", ex);
                throw;
            }
        }

        public void AddDriver(DateTime timestamp, string driverName)
        {
            try
            {
                var item = _data.GetOrAdd(timestamp, new DataItem { Timestamp = timestamp });
                item.DriverName = driverName;
                _databaseService.SaveDriver(timestamp, driverName);
                _loggingService.LogInformation($"Driver {driverName} added at {timestamp}");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error adding driver", ex);
                throw;
            }
        }

        public List<DataItem> GetAllData()
        {
            return _data.Values.OrderByDescending(x => x.Timestamp).ToList();
        }

        public void Clear()
        {
            try
            {
                _data.Clear();
                _databaseService.Clear();
                _loggingService.LogInformation("Data store cleared");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error clearing data store", ex);
                throw;
            }
        }
    }   
}