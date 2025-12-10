using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls; // Нужно для работы с Page/Frame
using Warehouse1.Data;
using Warehouse1.Services;
using Warehouse1.Session;
using Warehouse1.ViewModels;
using Warehouse1.Views;

namespace Warehouse1
{
    public partial class App : Application, IWindowService
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // --- ОБРАБОТКА ОШИБОК ---
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                MessageBox.Show("FATAL: " + ex.ExceptionObject.ToString());
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show("UI ERROR: " + ex.Exception.Message + "\n" + ex.Exception.StackTrace);
                ex.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, ex) =>
            {
                MessageBox.Show("TASK ERROR: " + ex.Exception.Message);
                ex.SetObserved();
            };

            // Приложение закрывается только когда закрывается главное окно
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var services = new ServiceCollection();

            // --- БАЗА ДАННЫХ ---
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Data Source=DESKTOP-JMVB6MO\\SQLEXPRESS;Initial Catalog=WarehouseDb;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));

            // --- СЕРВИСЫ ---
            services.AddSingleton<SecurityService>();
            services.AddTransient<WarehouseService>();
            services.AddTransient<AdminImportService>();
            services.AddTransient<ManagerService>();
            services.AddTransient<UserService>();
            services.AddSingleton<UserSession>();

            // WindowService (реализован в самом App)
            services.AddSingleton<IWindowService>(this);

            // --- VIEW MODELS ---
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AdminViewModel>();
            services.AddTransient<ManagerViewModel>();

            // --- ГЛАВНОЕ ОКНО И СТРАНИЦЫ (PAGES) ---
            // Регистрируем MainWindow (оболочка)
            services.AddSingleton<MainWindow>();

            // Регистрируем страницы
            services.AddTransient<LoginPage>();
            services.AddTransient<AdminPage>();
            services.AddTransient<ManagerPage>();

            // Seeder
            services.AddTransient<DbSeeder>();

            ServiceProvider = services.BuildServiceProvider();

            // --- ИНИЦИАЛИЗАЦИЯ БД ---
            try
            {
                var seeder = ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка инициализации БД: " + ex.Message);
            }

            // --- ЗАПУСК: ПОКАЗЫВАЕМ MAINWINDOW С LOGINPAGE ---
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            // Получаем страницу входа и её VM
            var loginPage = ServiceProvider.GetRequiredService<LoginPage>();
            loginPage.DataContext = ServiceProvider.GetRequiredService<LoginViewModel>();

            // Навигируем фрейм главного окна на страницу входа
            // ВАЖНО: В MainWindow.xaml должен быть Frame с x:Name="MainFrame"
            mainWindow.MainFrame.Navigate(loginPage);

            MainWindow = mainWindow;
            mainWindow.Show();
        }

        // 🟢 НОВАЯ ЛОГИКА НАВИГАЦИИ (SPA)
        public void CloseLoginAndOpen(string role, int userId)
        {
            try
            {
                // Получаем доступ к главному окну
                if (Application.Current.MainWindow is not MainWindow mainWindow)
                {
                    MessageBox.Show("Ошибка: Главное окно не найдено или имеет неверный тип.");
                    return;
                }

                // Устанавливаем сессию
                var session = ServiceProvider.GetRequiredService<UserSession>();
                session.CurrentUserId = userId;

                // Подготавливаем целевую страницу
                Page? targetPage = null;
                object? targetViewModel = null;

                if (role == "Admin")
                {
                    targetPage = ServiceProvider.GetRequiredService<AdminPage>();
                    targetViewModel = ServiceProvider.GetRequiredService<AdminViewModel>();
                }
                else
                {
                    targetPage = ServiceProvider.GetRequiredService<ManagerPage>();
                    targetViewModel = ServiceProvider.GetRequiredService<ManagerViewModel>();
                }

                // Привязываем ViewModel и переходим
                if (targetPage != null)
                {
                    targetPage.DataContext = targetViewModel;

                    // Просто меняем страницу внутри фрейма
                    mainWindow.MainFrame.Navigate(targetPage);

                    // Очищаем историю навигации, чтобы нельзя было вернуться "Назад" на логин (опционально)
                    // mainWindow.MainFrame.NavigationService.RemoveBackEntry();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при навигации: " + ex.Message);
            }
        }
    }
}