using System.Windows;
using System.Windows.Controls;
using Warehouse1.ViewModels;

namespace Warehouse1.Views
{
    /// <summary>
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {
        public ManagerPage()
        {
            InitializeComponent();
            this.Loaded += ManagerPage_Loaded;
        }

        private void ManagerPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManagerViewModel vm)
                vm.LoadWarehouses();
        }
    }
}
