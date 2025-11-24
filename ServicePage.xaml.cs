using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Trubachev41
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        private List<Product> allProducts; // Все продукты из базы
        private List<Product> currentServices; // Текущая отфильтрованная выборка
        private User currentUser;
        public ServicePage(User user)
        {
            InitializeComponent();
            currentUser = user;
            if (currentUser.UserID == 0 || currentUser.UserLogin == "guest")
            {
                FIOTB.Text = "Вы вошли как гость";
                RoleTB.Text = "Клиент";
            }
            else
            {
                FIOTB.Text = "Вы авторизованны как " + currentUser.UserSurname + " " + currentUser.UserName + " " + currentUser.UserPatronymic;
                switch (currentUser.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент"; break;
                    case 2:
                        RoleTB.Text = "Менеджер"; break;
                    case 3:
                        RoleTB.Text = "Администратор"; break;

                }
            }

            // Загружаем все продукты
            allProducts = Trubachev41Entities.GetContext().Product.ToList();
            currentServices = allProducts.ToList();

            ProductListView.ItemsSource = currentServices;
            ComboType.SelectedIndex = 0;

            // Обновляем счетчик при загрузке
            UpdateRecordsCount();
        }

        // Метод для обновления счетчика записей
        private void UpdateRecordsCount()
        {
            if (allProducts == null) return;

            int displayedCount = currentServices?.Count ?? 0;
            int totalCount = allProducts.Count;
            RecordsCountText.Text = $"кол-во {displayedCount} из {totalCount}";
        }

        private void UpdateServices()
        {
            // Начинаем с полного списка
            currentServices = allProducts.ToList();

            // Применяем фильтрацию по скидке
            if (ComboType.SelectedIndex == 0)
            {
                // "Все диапазоны" - не применяем фильтр
                currentServices = currentServices.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 0 && Convert.ToDouble(p.ProductDiscountAmount) <= 100)).ToList();
            }
            else if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 0 && Convert.ToDouble(p.ProductDiscountAmount) <= 9.99)).ToList();
            }
            else if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 10 && Convert.ToDouble(p.ProductDiscountAmount) <= 14.99)).ToList();
            }
            else if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 15 && Convert.ToDouble(p.ProductDiscountAmount) <= 100)).ToList();
            }

            // Применяем поиск
            currentServices = currentServices.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            // Применяем сортировку
            if (RButtonDown.IsChecked == true)
            {
                currentServices = currentServices.OrderByDescending(p => p.ProductCost).ToList();
            }
            else if (RButtonUp.IsChecked == true)
            {
                currentServices = currentServices.OrderBy(p => p.ProductCost).ToList();
            }

            // Обновляем ListView
            ProductListView.ItemsSource = currentServices;

            // Обновляем счетчик
            UpdateRecordsCount();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void TBoxCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Если у вас есть дополнительная функциональность для этого поля
        }
    }
}