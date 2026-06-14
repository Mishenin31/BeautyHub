using System.Windows;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class UsersView : UserControl
    {
        private readonly DataStore _data = DataStore.Instance;

        public UsersView()
        {
            InitializeComponent();
            Grid.ItemsSource = _data.Users;

            RoleBox.Items.Add(UserRole.Admin);
            RoleBox.Items.Add(UserRole.Employee);
            RoleBox.Items.Add(UserRole.User);
            RoleBox.SelectedIndex = 1; // по умолчанию Сотрудник
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            string login = LoginBox.Text.Trim();
            string pass = PassBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(pass) || RoleBox.SelectedItem is not UserRole role)
            {
                MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_data.LoginExists(login))
            {
                MessageBox.Show("Такой логин уже занят.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = new User
            {
                Id = _data.NextUserId(),
                FullName = name,
                Login = login,
                Password = pass,
                Role = role
            };

            // Для роли "Пользователь" заводим карточку клиента
            if (role == UserRole.User)
            {
                var client = new Client { Id = _data.NextClientId(), FullName = name, Phone = "" };
                _data.Clients.Add(client);
                user.Client = client;
            }

            _data.Users.Add(user);

            NameBox.Clear(); LoginBox.Clear(); PassBox.Clear();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is not User u)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            if (u == _data.CurrentUser)
            {
                MessageBox.Show("Нельзя удалить самого себя во время сеанса.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _data.Users.Remove(u);
        }
    }
}
