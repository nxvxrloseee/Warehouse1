using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Warehouse1.Models;
using Warehouse1.Services;
using Warehouse1.Session;
using Warehouse1.ViewModels.Base;

namespace Warehouse1.ViewModels
{
    public class ManagerViewModel : ViewModelsBase
    {
        private readonly WarehouseService _warehouseService;
        private readonly UserSession _session;
        public int currentUserId => _session.CurrentUserId;

        public ManagerViewModel(WarehouseService warehouseService, UserSession currentUserId)
        {
            _warehouseService = warehouseService;
            _session = currentUserId;

            SearchCommand = new RelayCommand(ExecuteSearch);
        }

        public ObservableCollection<ExternalTable1> Warehouses { get; } = new();

        private ExternalTable1? _selectedWarehouse;
        public ExternalTable1? SelectedWarehouse
        {
            get => _selectedWarehouse;
            set => Set(ref _selectedWarehouse, value);
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set => Set(ref _searchText, value);
        }

        private DataView? _searchResults;
        public DataView? SearchResults
        {
            get => _searchResults;
            set => Set(ref _searchResults, value);
        }

        public DataRowView? SelectedRow { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand TrackPriceCommand { get; }
        public ICommand ExportCsvCommand { get; }

        public async void LoadWarehouses()
        {
            try
            {
                Warehouses.Clear();
                var list = await _warehouseService.GetVirtualWarehousesAsync();
                foreach (var item in list) Warehouses.Add(item);

                if (Warehouses.Any())
                    SelectedWarehouse = Warehouses.First();
                else
                    MessageBox.Show("Склады не найдены");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки складов: " + ex.Message);
            }
        }

        private async void ExecuteSearch(object? p)
        {
            if (SelectedWarehouse == null) return;
            var dt = await _warehouseService.SearchProductAsync(SelectedWarehouse.Id, SearchText);
            SearchResults = dt.DefaultView;
        }
    }
}