using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using MechanotronicsApp.Data;
using MechanotronicsApp.Models;
using MechanotronicsApp.Interfaces;
using System.Linq;

namespace MechanotronicsApp
{
    public partial class MainWindow : Window
    {
        private readonly DataStore _dataStore;
        private readonly IDataGeneratorService _generatorService;
        private readonly ILoggingService _loggingService;
        private readonly IDatabaseService _databaseService;
        private readonly DispatcherTimer _timer;
        private readonly ObservableCollection<DataItem> _items;

        public MainWindow(DataStore dataStore, IDataGeneratorService generatorService, 
            ILoggingService loggingService, IDatabaseService databaseService)
        {
            InitializeComponent();
            _dataStore = dataStore;
            _generatorService = generatorService;
            _loggingService = loggingService;
            _databaseService = databaseService;

            _items = new ObservableCollection<DataItem>();
            MainDataGrid.ItemsSource = _items;

            LoadInitialData();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            _loggingService.LogInformation("MainWindow initialized");
        }

        private void LoadInitialData()
        {
            try
            {
                var data = _databaseService.GetAllData();
                foreach (var item in data)
                {
                    _items.Add(item);
                }
                _loggingService.LogInformation($"Loaded {data.Count} items from database");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error loading initial data from database", ex);
                MessageBox.Show("Ошибка при загрузке данных из базы данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            try
            {
                var data = _dataStore.GetAllData();
                var newItems = data.Where(item => !_items.Any(existingItem => 
                    existingItem.Timestamp == item.Timestamp));

                foreach (var item in newItems)
                {
                    _items.Insert(0, item);
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error refreshing data", ex);
                StatusText.Text = "Ошибка обновления данных";
            }
        }

        private async void StartCarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _generatorService.StartCarGenerationAsync(default);
                _loggingService.LogInformation("Car generation started");
                StatusText.Text = "Генерация автомобилей запущена";
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error starting car generation", ex);
                MessageBox.Show("Ошибка при запуске генерации автомобилей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopCarButton_Click(object sender, RoutedEventArgs e)
        {
            _generatorService.StopCarGeneration();
            _loggingService.LogInformation("Car generation stopped");
            StatusText.Text = "Генерация автомобилей остановлена";
        }

        private async void StartDriverButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _generatorService.StartDriverGenerationAsync(default);
                _loggingService.LogInformation("Driver generation started");
                StatusText.Text = "Генерация водителей запущена";
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error starting driver generation", ex);
                MessageBox.Show("Ошибка при запуске генерации водителей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopDriverButton_Click(object sender, RoutedEventArgs e)
        {
            _generatorService.StopDriverGeneration();
            _loggingService.LogInformation("Driver generation stopped");
            StatusText.Text = "Генерация водителей остановлена";
        }

        private void OpenDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var detailsWindow = new DetailsWindow(_dataStore, _loggingService, _databaseService);
                detailsWindow.Show();
                _loggingService.LogInformation("Details window opened");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error opening details window", ex);
                MessageBox.Show("Ошибка при открытии окна деталей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            _loggingService.LogInformation("MainWindow closed");
            base.OnClosed(e);
        }
    }
} 