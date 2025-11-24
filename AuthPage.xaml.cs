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
using System.Windows.Shapes;

namespace Trubachev41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void guest_Click(object sender, RoutedEventArgs e)
        {
            User guestUser = new User
            {
                UserID = 0,
                UserLogin = "guest", UserPassword = "",
                UserName = "Гость", UserSurname = "", UserPatronymic = "",
                UserRole = 1
            };
            Manager.MainFrame.Navigate(new ServicePage(guestUser));
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            string login=LoginTB.Text;
            string password= PassTB.Text;
            if(login == ""||password=="")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user=Trubachev41Entities.GetContext().User.ToList().Find(p=>p.UserLogin==login && p.UserPassword==password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ServicePage(user));
                LoginTB.Text = "";
                PassTB.Text = "";
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                LoginBtn.IsEnabled=false;
                LoginBtn.IsEnabled=true;
            }
        }

        private void LoginTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PassTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
