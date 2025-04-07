using System;
using System.Windows;
using System.Windows.Controls;
using MechanotronicsApp.Data;
using MechanotronicsApp.Models;
using MechanotronicsApp.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MechanotronicsApp
{
    public partial class DetailsWindow : Window
    {
        private readonly IDataStore _dataStore;
        private readonly ILoggingService _loggingService;
        private readonly IDatabaseService _databaseService;
        private readonly List<DataItem> _allData;
        private DetailsTableWindow _detailsTableWindow;

        public DetailsWindow(IDataStore dataStore, ILoggingService loggingService, IDatabaseService databaseService)
        {
            InitializeComponent();
            _dataStore = dataStore;
            _loggingService = loggingService;
            _databaseService = databaseService;

            try
            {
                _allData = _databaseService.GetAllData();
                UpdateDataGrid();
                _loggingService.LogInformation("Details window initialized with data");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error initializing details window", ex);
                MessageBox.Show("Ошибка при загрузке данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDataGrid()
        {
            try
            {
                var groupedData = _allData
                    .GroupBy(item => item.Timestamp.Date)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Cars = g.Count(item => !string.IsNullOrEmpty(item.CarName)),
                        Drivers = g.Count(item => !string.IsNullOrEmpty(item.DriverName))
                    });

                DetailsDataGrid.ItemsSource = groupedData;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error updating data grid", ex);
                MessageBox.Show("Ошибка при обновлении данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _allData.Clear();
                _allData.AddRange(_databaseService.GetAllData());
                UpdateDataGrid();
                _loggingService.LogInformation("Data refreshed in details window");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error refreshing data", ex);
                MessageBox.Show("Ошибка при обновлении данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataStore.Clear();
                _allData.Clear();
                UpdateDataGrid();
                _loggingService.LogInformation("Data cleared in details window");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error clearing data", ex);
                MessageBox.Show("Ошибка при очистке данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenTableButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_detailsTableWindow == null || !_detailsTableWindow.IsLoaded)
                {
                    _detailsTableWindow = new DetailsTableWindow(_dataStore, _loggingService, _databaseService);
                    _detailsTableWindow.Show();
                    _loggingService.LogInformation("Details table window opened");
                }
                else
                {
                    _detailsTableWindow.Activate();
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error opening details table window", ex);
                MessageBox.Show("Ошибка при открытии детальной таблицы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 
