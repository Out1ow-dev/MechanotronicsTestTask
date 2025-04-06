using System;
using System.Collections.Generic;
using MechanotronicsApp.Models;

namespace MechanotronicsApp.Interfaces
{
    public interface IDataStore
    {
        void AddCar(DateTime timestamp, string carName);
        void AddDriver(DateTime timestamp, string driverName);
        List<DataItem> GetAllData();
        void Clear();
    }
} 