using System.Windows;
using System.Windows.Controls;

namespace BeautyHub.Views
{
    public partial class LoginWindow : Window
    {
        private readonly DataStore _data = DataStore.Instance;

        public LoginWindow()
        {
            InitializeComponent();
        }

        // --- Переключение вкладок ---
        private void ShowLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
            TabLoginBtn.Background = (System.Windows.Media.Brush)FindResource("PrimaryBrush");
            TabRegisterBtn.Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(0xBB, 0xBB, 0xBB));
        }

        private void ShowRegister_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
            TabRegisterBtn.Background = (System.Windows.Media.Brush)FindResource("PrimaryBrush");
            TabLoginBtn.Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(0xBB, 0xBB, 0xBB));
        }

        // --- Вход ---
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginName.Text.Trim();
            string password = LoginPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _data.Authenticate(login, password);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _data.CurrentUser = user;
            OpenMainWindow();
        }

        // --- Регистрация ---
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string name = RegName.Text.Trim();
            string phone = RegPhone.Text.Trim();
            string login = RegLogin.Text.Trim();
            string pass = RegPassword.Password;
            string pass2 = RegPassword2.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Заполните ФИО, логин и пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pass != pass2)
            {
                MessageBox.Show("Пароли не совпадают.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_data.LoginExists(login))
            {
                MessageBox.Show("Такой логин уже занят.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _data.RegisterUser(login, pass, name, phone);
            _data.CurrentUser = user;

            MessageBox.Show("Регистрация прошла успешно!", "Beauty Hub", MessageBoxButton.OK, MessageBoxImage.Information);
            OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            var main = new MainWindow();
            main.Show();
            Close();
        }
    }
}
