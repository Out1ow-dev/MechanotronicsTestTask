using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using Moq;
using MechanotronicsApp.Services;
using MechanotronicsApp.Interfaces;
using MechanotronicsApp.Models;
using MechanotronicsApp.Data;

namespace MechanotronicsTests
{
    public class DataGeneratorServiceTests
    {
        private readonly DataStore _dataStore;
        private readonly Mock<IDatabaseService> _databaseServiceMock;
        private readonly Mock<ILoggingService> _loggingServiceMock;
        private readonly DataGeneratorService _service;

        public DataGeneratorServiceTests()
        {
            _databaseServiceMock = new Mock<IDatabaseService>();
            _loggingServiceMock = new Mock<ILoggingService>();
            _dataStore = new DataStore(_databaseServiceMock.Object, _loggingServiceMock.Object);
            _service = new DataGeneratorService(_dataStore, _loggingServiceMock.Object);
        }

        [Fact]
        public async Task StartCarGeneration_ShouldGenerateCarsEvery2Seconds()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var cars = new System.Collections.Generic.List<DataItem>();

            _databaseServiceMock.Setup(x => x.SaveCar(It.IsAny<DateTime>(), It.IsAny<string>()))
                .Callback<DateTime, string>((timestamp, carName) =>
                {
                    cars.Add(new DataItem { Timestamp = timestamp, CarName = carName });
                });

            // Act
            await _service.StartCarGenerationAsync(cts.Token);
            await Task.Delay(1000); 
            await _service.StopCarGeneration();
            cts.Cancel();

            // Assert
            Assert.Equal(1, cars.Count); 
            Assert.All(cars, item => Assert.NotNull(item.CarName));
            Assert.All(cars, item => Assert.Null(item.DriverName));
        }

        [Fact]
        public async Task StartDriverGeneration_ShouldGenerateDriversEvery3Seconds()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var drivers = new System.Collections.Generic.List<DataItem>();

            _databaseServiceMock.Setup(x => x.SaveDriver(It.IsAny<DateTime>(), It.IsAny<string>()))
                .Callback<DateTime, string>((timestamp, driverName) =>
                {
                    drivers.Add(new DataItem { Timestamp = timestamp, DriverName = driverName });
                });

            // Act
            await _service.StartDriverGenerationAsync(cts.Token);
            await Task.Delay(1000); 
            await _service.StopDriverGeneration();
            cts.Cancel();

            // Assert
            Assert.Equal(1, drivers.Count); 
            Assert.All(drivers, item => Assert.NotNull(item.DriverName));
            Assert.All(drivers, item => Assert.Null(item.CarName));
        }

        [Fact]
        public async Task StartBothGenerations_ShouldGenerateBothTypes()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var items = new System.Collections.Generic.List<DataItem>();

            _databaseServiceMock.Setup(x => x.SaveCar(It.IsAny<DateTime>(), It.IsAny<string>()))
                .Callback<DateTime, string>((timestamp, carName) =>
                {
                    items.Add(new DataItem { Timestamp = timestamp, CarName = carName });
                });

            _databaseServiceMock.Setup(x => x.SaveDriver(It.IsAny<DateTime>(), It.IsAny<string>()))
                .Callback<DateTime, string>((timestamp, driverName) =>
                {
                    items.Add(new DataItem { Timestamp = timestamp, DriverName = driverName });
                });

            // Act
            await _service.StartCarGenerationAsync(cts.Token);
            await _service.StartDriverGenerationAsync(cts.Token);
            await Task.Delay(1000); 
            await _service.StopCarGeneration();
            await _service.StopDriverGeneration();
            cts.Cancel();

            // Assert
            var cars = items.Count(x => x.CarName != null);
            var drivers = items.Count(x => x.DriverName != null);
            Assert.Equal(1, cars); 
            Assert.Equal(1, drivers); 
        }

        [Fact]
        public async Task StopGeneration_ShouldStopGenerating()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var items = new System.Collections.Generic.List<DataItem>();

            _databaseServiceMock.Setup(x => x.SaveCar(It.IsAny<DateTime>(), It.IsAny<string>()))
                .Callback<DateTime, string>((timestamp, carName) =>
                {
                    items.Add(new DataItem { Timestamp = timestamp, CarName = carName });
                });

            // Act
            await _service.StartCarGenerationAsync(cts.Token);
            await Task.Delay(1000); 
            await _service.StopCarGeneration();
            await Task.Delay(1000); 

            // Assert
            Assert.Equal(1, items.Count); 
        }

        [Fact]
        public async Task StartCarGeneration_WhenAlreadyRunning_ShouldNotStartAgain()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            await _service.StartCarGenerationAsync(cts.Token);

            // Act
            await _service.StartCarGenerationAsync(cts.Token);

            // Assert
            _loggingServiceMock.Verify(x => x.LogWarning("Car generation is already running"), Times.Once);
        }

        [Fact]
        public async Task StartDriverGeneration_WhenAlreadyRunning_ShouldNotStartAgain()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            await _service.StartDriverGenerationAsync(cts.Token);

            // Act
            await _service.StartDriverGenerationAsync(cts.Token);

            // Assert
            _loggingServiceMock.Verify(x => x.LogWarning("Driver generation is already running"), Times.Once);
        }

        [Fact]
        public async Task StopCarGeneration_WhenNotRunning_ShouldNotThrowException()
        {
            // Act
            await _service.StopCarGeneration();

            // Assert
            _loggingServiceMock.Verify(x => x.LogInformation("Car generation stopped"), Times.Never);
        }

        [Fact]
        public async Task StopDriverGeneration_WhenNotRunning_ShouldNotThrowException()
        {
            // Act
            await _service.StopDriverGeneration();

            // Assert
            _loggingServiceMock.Verify(x => x.LogInformation("Driver generation stopped"), Times.Never);
        }

        [Fact]
        public void Dispose_ShouldCleanupResources() // после диспоза генерация не запускается
        {
            // Act
            _service.Dispose();

            // Assert           
            var cts = new CancellationTokenSource();
            Assert.ThrowsAsync<ObjectDisposedException>(() => 
                _service.StartCarGenerationAsync(cts.Token));
        }
    }
} 