using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using Warehouse1.Services;
using Warehouse1.ViewModels.Base;
using Warehouse1.Session;
using System.Collections.ObjectModel;
using Warehouse1.Models;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Warehouse1.ViewModels
{
    public class AdminViewModel : ViewModelsBase
    {
        private readonly AdminImportService _importService;
        private readonly UserService _userService;
        private readonly UserSession _session;

        public int currentUserId => _session.CurrentUserId;

        public AdminViewModel(AdminImportService importService, UserService userService, UserSession currentUserId)
        {
            _importService = importService;
            _session = currentUserId;
            _userService = userService;


            ImportCommand = new RelayCommand(ExecuteImport);
            // Команды пользователей
            AddUserCommand = new RelayCommand(ExecuteAddUser);
            EditUserCommand = new RelayCommand(ExecuteEditUser);
            DeleteUserCommand = new RelayCommand(ExecuteDeleteUser);

            // Команды настроек
            ToggleThemeCommand = new RelayCommand(ExecuteToggleTheme);
            SetFontCommand = new RelayCommand(ExecuteSetFont);

            LoadUsers();

        }

        public ObservableCollection<User> Users { get; } = new();

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                Set(ref _selectedUser, value);
                if (value != null)
                {
                    InputUsername = value.Username;
                    InputRole = value.UserRoles.FirstOrDefault()?.Role.Name ?? "Manager";
                }
            }
        }

        private string _inputUsername = "";
        public string InputUsername { get => _inputUsername; set => Set(ref _inputUsername, value); }

        private string _inputPassword = "";
        public string InputPassword { get => _inputPassword; set => Set(ref _inputPassword, value); } // Для нового пароля

        private string _inputRole = "Manager";
        public string InputRole { get => _inputRole; set => Set(ref _inputRole, value); }

        public ObservableCollection<string> Roles { get; } = new() { "Admin", "Manager" };

        public ICommand ImportCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand SetFontCommand { get; }

        private async void LoadUsers()
        {
            Users.Clear();
            var list = await _userService.GetAllUsersAsync();
            foreach (var u in list) Users.Add(u);
        }

        private async void ExecuteAddUser(object? p)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputUsername) || string.IsNullOrWhiteSpace(InputPassword))
                {
                    MessageBox.Show("Заполните логин и пароль");
                    return;
                }
                await _userService.CreateUserAsync(InputUsername, InputPassword, InputRole);
                LoadUsers();
                MessageBox.Show("Пользователь создан");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void ExecuteEditUser(object? p)
        {
            if (SelectedUser == null) return;
            try
            {
                await _userService.UpdateUserAsync(SelectedUser, InputPassword, InputRole);
                LoadUsers();
                MessageBox.Show("Пользователь обновлен");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void ExecuteDeleteUser(object? p)
        {
            if (SelectedUser == null) return;
            if (SelectedUser.Id == _session.CurrentUserId)
            {
                MessageBox.Show("Нельзя удалить самого себя!");
                return;
            }

            if (MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _userService.DeleteUserAsync(SelectedUser.Id);
                LoadUsers();
            }
        }

        private async void ExecuteImport(object? p)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Выберите файл для импорта"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string tableName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя новой таблицы (на латинице):", "Имя таблицы", "NewTable");

                if (string.IsNullOrWhiteSpace(tableName)) return;

                try
                {
                    await _importService.ImportExcelTableAsync(openFileDialog.FileName, tableName, currentUserId);
                    MessageBox.Show("Таблица успешно импортирована!");
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"Ошибка импорта: {ex.Message}");
                }
            }
        }

        private bool _isDarkTheme = false;
        private void ExecuteToggleTheme(object? p)
        {
            _isDarkTheme = !_isDarkTheme;
            var app = (App)Application.Current;

            if (_isDarkTheme)
            {
                app.Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(30,30, 30));
                app.Resources["TextColor"] = new SolidColorBrush(Colors.White);
            }
            else
            {
                app.Resources["WindowBackground"] = new SolidColorBrush(Colors.White);
                app.Resources["TextColor"] = new SolidColorBrush(Colors.Black);
            }
        }

        private void ExecuteSetFont(object? p)
        {
            Application.Current.MainWindow.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), ".Resources/#Jetbrains");
            MessageBox.Show("Шрифт был изменён");
        }
    }
}
