using System;
using System.Threading;
using System.Threading.Tasks;
using MechanotronicsApp.Data;
using MechanotronicsApp.Interfaces;
using MechanotronicsApp.Models;

namespace MechanotronicsApp.Services
{
    public class DataGeneratorService : IDataGeneratorService, IDisposable
    {
        private readonly DataStore _dataStore;
        private readonly ILoggingService _loggingService;
        private readonly SemaphoreSlim _carSemaphore = new(1, 1);
        private readonly SemaphoreSlim _driverSemaphore = new(1, 1);
        private CancellationTokenSource _carCts = new();
        private CancellationTokenSource _driverCts = new();
        private bool _isDisposed;

        private readonly string[] _carNames = new[]
        {
            "Мондео", "Крета", "Приус", "УАЗик", "Вольво",
            "Фокус", "Октавия", "Запорожец"
        };

        private readonly string[] _driverNames = new[]
        {
            "Петр", "Василий", "Николай", "Марина", "Феодосий", "Карина"
        };

        private int _currentCarIndex = -1;
        private int _currentDriverIndex = -1;

        public DataGeneratorService(DataStore dataStore, ILoggingService loggingService)
        {
            _dataStore = dataStore;
            _loggingService = loggingService;
        }

        private string GetNextCar()
        {
            _currentCarIndex = (_currentCarIndex + 1) % _carNames.Length;
            return _carNames[_currentCarIndex];
        }

        private string GetNextDriver()
        {
            _currentDriverIndex = (_currentDriverIndex + 1) % _driverNames.Length;
            return _driverNames[_currentDriverIndex];
        }

        public async Task StartCarGenerationAsync(CancellationToken cancellationToken)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(DataGeneratorService));

            if (!await _carSemaphore.WaitAsync(0))
            {
                _loggingService.LogWarning("Car generation is already running");
                return;
            }

            try
            {
                _carCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (!_carCts.Token.IsCancellationRequested)
                        {
                            var timestamp = DateTime.Now;
                            var carName = GetNextCar();
                            _dataStore.AddCar(timestamp, carName);
                            await Task.Delay(2000, _carCts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        _loggingService.LogError("Error in car generation task", ex);
                    }
                    finally
                    {
                        _carSemaphore.Release();
                    }
                }, _carCts.Token);
            }
            catch (Exception ex)
            {
                _carSemaphore.Release();
                _loggingService.LogError("Error starting car generation", ex);
                throw;
            }
        }

        public async Task StartDriverGenerationAsync(CancellationToken cancellationToken)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(DataGeneratorService));

            if (!await _driverSemaphore.WaitAsync(0))
            {
                _loggingService.LogWarning("Driver generation is already running");
                return;
            }

            try
            {
                _driverCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (!_driverCts.Token.IsCancellationRequested)
                        {
                            var timestamp = DateTime.Now;
                            var driverName = GetNextDriver();
                            _dataStore.AddDriver(timestamp, driverName);
                            await Task.Delay(3000, _driverCts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        _loggingService.LogError("Error in driver generation task", ex);
                    }
                    finally
                    {
                        _driverSemaphore.Release();
                    }
                }, _driverCts.Token);
            }
            catch (Exception ex)
            {
                _driverSemaphore.Release();
                _loggingService.LogError("Error starting driver generation", ex);
                throw;
            }
        }

        public async Task StopCarGeneration()
        {
            if (_isDisposed)
                return;

            _carCts.Cancel();
            try
            {
                await _carSemaphore.WaitAsync();
                _carSemaphore.Release();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public async Task StopDriverGeneration()
        {
            if (_isDisposed)
                return;

            _driverCts.Cancel();
            try
            {
                await _driverSemaphore.WaitAsync();
                _driverSemaphore.Release();
            }
            catch (ObjectDisposedException)
            {

            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            _carCts?.Cancel();
            _carCts?.Dispose();
            _driverCts?.Cancel();
            _driverCts?.Dispose();

            _carSemaphore.Dispose();
            _driverSemaphore.Dispose();
        }
    }
} 