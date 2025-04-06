using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MechanotronicsApp.Models;

namespace MechanotronicsApp.Interfaces
{
    public interface IDatabaseService
    {
        void SaveCar(DateTime timestamp, string carName);
        void SaveDriver(DateTime timestamp, string driverName);
        Task AddDataItemAsync(DataItem item);
        List<DataItem> GetAllData();
        void Clear();
    }
} 