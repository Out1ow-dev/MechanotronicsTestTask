using System;
using System.Windows;
using System.Windows.Threading;
using MechanotronicsApp.Data;
using MechanotronicsApp.Models;
using MechanotronicsApp.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MechanotronicsApp
{
    public partial class DetailsTableWindow : Window
    {
        private readonly IDataStore _dataStore;
        private readonly ILoggingService _loggingService;
        private readonly IDatabaseService _databaseService;
        private readonly DispatcherTimer _timer;
        private readonly List<DataItem> _allData;

        public DetailsTableWindow(IDataStore dataStore, ILoggingService loggingService, IDatabaseService databaseService)
        {
            InitializeComponent();
            _dataStore = dataStore;
            _loggingService = loggingService;
            _databaseService = databaseService;

            _allData = new List<DataItem>();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            LoadInitialData();
            _loggingService.LogInformation("Details table window initialized");
        }

        private void LoadInitialData()
        {
            try
            {
                _allData.Clear();
                _allData.AddRange(_databaseService.GetAllData());
                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error loading initial data", ex);
                MessageBox.Show("Ошибка при загрузке данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                var newData = _databaseService.GetAllData();
                var hasNewData = false;

                foreach (var item in newData)
                {
                    if (!_allData.Any(existing => existing.Timestamp == item.Timestamp))
                    {
                        _allData.Add(item);
                        hasNewData = true;
                    }
                }

                if (hasNewData)
                {
                    UpdateDataGrid();
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error updating data", ex);
            }
        }

        private void UpdateDataGrid()
        {
            try
            {
                var sortedData = _allData.OrderByDescending(item => item.Timestamp).ToList();
                DetailsTableDataGrid.ItemsSource = sortedData;

                if (sortedData.Any())
                {
                    DetailsTableDataGrid.ScrollIntoView(sortedData[0]);
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error updating data grid", ex);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            _loggingService.LogInformation("Details table window closed");
            base.OnClosed(e);
        }
    }
} 