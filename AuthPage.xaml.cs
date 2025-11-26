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
using System.Windows.Threading;

namespace Trubachev41
{
    public partial class AuthPage : Page
    {
        private string currentCaptcha;
        private int failedAttempts = 0;
        private DispatcherTimer blockTimer;
        private int blockTimeSeconds = 0;
        private bool isCaptchaRequired = false;

        public AuthPage()
        {
            InitializeComponent();
            InitializeBlockTimer();
            HideCaptcha();
        }

        private void InitializeBlockTimer()
        {
            blockTimer = new DispatcherTimer();
            blockTimer.Interval = TimeSpan.FromSeconds(1);
            blockTimer.Tick += BlockTimer_Tick;
        }

        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            blockTimeSeconds--;

            if (blockTimeSeconds <= 0)
            {
                blockTimer.Stop();
                EnableLoginControls();
                BlockTimerText.Text = "";
            }
            else
            {
                BlockTimerText.Text = $"Блокировка: {blockTimeSeconds} сек.";
            }
        }

        private void GenerateCaptcha()
        {
            currentCaptcha = GenerateRandomCaptcha(4);

            // Отображаем каждый символ капчи в отдельном TextBlock с разными эффектами
            if (currentCaptcha.Length >= 4)
            {
                captchaOneWord.Text = currentCaptcha[0].ToString();
                captchaTwoWord.Text = currentCaptcha[1].ToString();
                captchaThreeWord.Text = currentCaptcha[2].ToString();
                captchaFourWord.Text = currentCaptcha[3].ToString();
            }

            ShowCaptcha();
        }

        private string GenerateRandomCaptcha(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

            var code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }
            return new string(code);
        }

        private void ShowCaptcha()
        {
            isCaptchaRequired = true;
            captchaOneWord.Visibility = Visibility.Visible;
            captchaTwoWord.Visibility = Visibility.Visible;
            captchaThreeWord.Visibility = Visibility.Visible;
            captchaFourWord.Visibility = Visibility.Visible;
            CaptchaInputBox.Visibility = Visibility.Visible;
            CaptchaInputBox.Text = "";
            CaptchaInputBox.Focus();
        }

        private void HideCaptcha()
        {
            isCaptchaRequired = false;
            captchaOneWord.Visibility = Visibility.Hidden;
            captchaTwoWord.Visibility = Visibility.Hidden;
            captchaThreeWord.Visibility = Visibility.Hidden;
            captchaFourWord.Visibility = Visibility.Hidden;
            CaptchaInputBox.Visibility = Visibility.Hidden;
            CaptchaInputBox.Text = "";
        }

        private void guest_Click(object sender, RoutedEventArgs e)
        {
            

            User guestUser = new User
            {
                UserID = 0,
                UserLogin = "guest",
                UserPassword = "",
                UserName = "Гость",
                UserSurname = "",
                UserPatronymic = "",
                UserRole = 1
            };
            Manager.MainFrame.Navigate(new ServicePage(guestUser));
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (blockTimer.IsEnabled) return; // Заблокировано

            string login = LoginTB.Text;
            string password = PassTB.Text;

            if (login == "" || password == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            // Проверка капчи, если требуется
            if (isCaptchaRequired)
            {
                string captchaInput = CaptchaInputBox.Text;
                if (string.IsNullOrEmpty(captchaInput))
                {
                    MessageBox.Show("Введите капчу");
                    return;
                }

                if (!ValidateCaptcha(captchaInput))
                {
                    MessageBox.Show("Неверно введена капча");
                    GenerateCaptcha();
                    BlockLoginControls(10); // Блокировка на 10 секунд
                    return;
                }
            }

            User user = Trubachev41Entities.GetContext().User.ToList()
                .Find(p => p.UserLogin == login && p.UserPassword == password);

            if (user != null)
            {
                // Сброс счетчика при успешной авторизации
                failedAttempts = 0;
                HideCaptcha();
                Manager.MainFrame.Navigate(new ServicePage(user));
                LoginTB.Text = "";
                PassTB.Text = "";
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                failedAttempts++;

                // После первой ошибки показываем капчу
                if (failedAttempts >= 1 && !isCaptchaRequired)
                {
                    GenerateCaptcha();
                }
                else if (isCaptchaRequired)
                {
                    GenerateCaptcha(); // Обновляем капчу при повторной ошибке
                }
            }
        }

        private bool ValidateCaptcha(string input)
        {
            return !string.IsNullOrEmpty(currentCaptcha) &&
                   currentCaptcha.Equals(input, StringComparison.OrdinalIgnoreCase);
        }

        private void BlockLoginControls(int seconds)
        {
            blockTimeSeconds = seconds;
            LoginBtn.IsEnabled = false;
            
            BlockTimerText.Text = $"Блокировка: {blockTimeSeconds} сек.";
            blockTimer.Start();
        }

        private void EnableLoginControls()
        {
            LoginBtn.IsEnabled = true;
            GuestBtn.IsEnabled = true;
            GenerateCaptcha(); // Генерируем новую капчу после разблокировки
        }

        private void LoginTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Дополнительная логика при изменении логина
        }

        private void PassTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Дополнительная логика при изменении пароля
        }
    }
}