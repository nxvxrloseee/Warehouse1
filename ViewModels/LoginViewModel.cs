using System.Windows;
using System.Windows.Input;
using Warehouse1.Models;
using Warehouse1.Services;
using Warehouse1.ViewModels.Base;

namespace Warehouse1.ViewModels
{
    public class LoginViewModel : ViewModelsBase
    {
        private readonly SecurityService _securityService;
        private readonly IWindowService _windowService;

        public LoginViewModel(SecurityService securityService, IWindowService windowService)
        {
            _securityService = securityService;
            _windowService = windowService;
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        public string Username { get; set; } = "admin"; // Для удобства тестов

        public ICommand LoginCommand { get; }

        private async void ExecuteLogin(object? parameter)
        {
            if (parameter is not System.Windows.Controls.PasswordBox passBox) return;

            try
            {
                // Теперь ожидаем LoginResult!
                var result = await _securityService.LoginAsync(Username, passBox.Password);

                // Проверяем через IsSuccess
                if (!result.IsSuccess)
                {
                    // Используем ErrorMessage из LoginResult
                    MessageBox.Show(result.ErrorMessage ?? "Неверные данные");
                    return;
                }

                // Доступ к объекту User через .User (он гарантированно не null, если IsSuccess true)
                var user = result.User!;

                string role = _securityService.IsAdmin(user) ? "Admin" : "Manager";

                // Это вызывает IWindowService.CloseLoginAndOpen, который должен быть обновлен (см. ниже)
                _windowService.CloseLoginAndOpen(role, user.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }
    }
}