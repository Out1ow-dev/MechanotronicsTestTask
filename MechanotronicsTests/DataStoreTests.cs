using System;
using System.Linq;
using Xunit;
using Moq;
using MechanotronicsApp.Models;
using MechanotronicsApp.Data;
using MechanotronicsApp.Interfaces;

namespace MechanotronicsTests
{
    public class DataStoreTests
    {
        private readonly DataStore _dataStore;
        private readonly Mock<IDatabaseService> _databaseServiceMock;
        private readonly Mock<ILoggingService> _loggingServiceMock;

        public DataStoreTests()
        {
            _databaseServiceMock = new Mock<IDatabaseService>();
            _loggingServiceMock = new Mock<ILoggingService>();
            _dataStore = new DataStore(_databaseServiceMock.Object, _loggingServiceMock.Object);
        }

        [Fact]
        public void AddCar_ShouldAddCarToStore()
        {
            // Arrange
            var timestamp = DateTime.Now;
            var carName = "Test Car";

            // Act
            _dataStore.AddCar(timestamp, carName);

            // Assert
            _databaseServiceMock.Verify(x => x.SaveCar(timestamp, carName), Times.Once);
        }

        [Fact]
        public void AddDriver_ShouldAddDriverToStore()
        {
            // Arrange
            var timestamp = DateTime.Now;
            var driverName = "Test Driver";

            // Act
            _dataStore.AddDriver(timestamp, driverName);

            // Assert
            _databaseServiceMock.Verify(x => x.SaveDriver(timestamp, driverName), Times.Once);
        }

        [Fact]
        public void AddCarAndDriver_WithSameTimestamp_ShouldCombineData()
        {
            // Arrange
            var timestamp = DateTime.Now;
            var carName = "Test Car";
            var driverName = "Test Driver";

            // Act
            _dataStore.AddCar(timestamp, carName);
            _dataStore.AddDriver(timestamp, driverName);

            // Assert
            var allData = _dataStore.GetAllData();
            var matchingData = allData.FirstOrDefault(x => x.Timestamp == timestamp);
            Assert.NotNull(matchingData);
            Assert.Equal(carName, matchingData.CarName);
            Assert.Equal(driverName, matchingData.DriverName);
        }

        [Fact]
        public void GetAllData_ShouldReturnDataInDescendingOrder()
        {
            // Arrange
            var timestamp1 = DateTime.Now.AddMinutes(-2);
            var timestamp2 = DateTime.Now.AddMinutes(-1);
            var timestamp3 = DateTime.Now;

            _dataStore.AddCar(timestamp1, "Car 1");
            _dataStore.AddCar(timestamp2, "Car 2");
            _dataStore.AddCar(timestamp3, "Car 3");

            // Act
            var allData = _dataStore.GetAllData().ToList();

            // Assert
            Assert.Equal(3, allData.Count);
            Assert.Equal(timestamp3, allData[0].Timestamp);
            Assert.Equal(timestamp2, allData[1].Timestamp);
            Assert.Equal(timestamp1, allData[2].Timestamp);
        }

        [Fact]
        public void Clear_ShouldRemoveAllData()
        {
            // Arrange
            _dataStore.AddCar(DateTime.Now, "Test Car");
            _dataStore.AddDriver(DateTime.Now, "Test Driver");

            // Act
            _dataStore.Clear();

            // Assert
            Assert.Empty(_dataStore.GetAllData());
            _databaseServiceMock.Verify(x => x.Clear(), Times.Once);
        }

        [Fact]
        public void AddCar_WhenDatabaseThrowsException_ShouldPropagateException()
        {
            // Arrange
            var timestamp = DateTime.Now;
            var carName = "Test Car";
            var expectedException = new Exception("Database error");
            _databaseServiceMock.Setup(x => x.SaveCar(timestamp, carName))
                .Throws(expectedException);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _dataStore.AddCar(timestamp, carName));
            Assert.Same(expectedException, exception);
            _loggingServiceMock.Verify(x => x.LogError("Error adding car", expectedException), Times.Once);
        }

        [Fact]
        public void AddDriver_WhenDatabaseThrowsException_ShouldPropagateException()
        {
            // Arrange
            var timestamp = DateTime.Now;
            var driverName = "Test Driver";
            var expectedException = new Exception("Database error");
            _databaseServiceMock.Setup(x => x.SaveDriver(timestamp, driverName))
                .Throws(expectedException);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _dataStore.AddDriver(timestamp, driverName));
            Assert.Same(expectedException, exception);
            _loggingServiceMock.Verify(x => x.LogError("Error adding driver", expectedException), Times.Once);
        }

        [Fact]
        public void Clear_WhenDatabaseThrowsException_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new Exception("Database error");
            _databaseServiceMock.Setup(x => x.Clear())
                .Throws(expectedException);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _dataStore.Clear());
            Assert.Same(expectedException, exception);
            _loggingServiceMock.Verify(x => x.LogError("Error clearing data store", expectedException), Times.Once);
        }
    }
} 