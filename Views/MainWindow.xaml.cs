using System.Windows;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class MainWindow : Window
    {
        private readonly DataStore _data = DataStore.Instance;

        public MainWindow()
        {
            InitializeComponent();

            var user = _data.CurrentUser;
            if (user == null)
            {
                // Защита: без авторизации возвращаемся на экран входа
                new LoginWindow().Show();
                Close();
                return;
            }

            UserNameText.Text = user.FullName;
            UserRoleText.Text = user.RoleDisplay;

            BuildMenu(user.Role);
        }

        // Формирование пунктов меню в зависимости от роли
        private void BuildMenu(UserRole role)
        {
            NavPanel.Children.Clear();

            switch (role)
            {
                case UserRole.Admin:
                    AddNav("Записи", (s, e) => Show(new AppointmentsView()));
                    AddNav("Услуги", (s, e) => Show(new ServicesView()));
                    AddNav("Мастера", (s, e) => Show(new MastersView()));
                    AddNav("Клиенты", (s, e) => Show(new ClientsView()));
                    AddNav("Пользователи", (s, e) => Show(new UsersView()));
                    AddNav("Статистика", (s, e) => Show(new StatsView()));
                    Show(new AppointmentsView());
                    break;

                case UserRole.Employee:
                    AddNav("Записи", (s, e) => Show(new AppointmentsView()));
                    AddNav("Услуги", (s, e) => Show(new ServicesView()));
                    AddNav("Мастера", (s, e) => Show(new MastersView()));
                    AddNav("Клиенты", (s, e) => Show(new ClientsView()));
                    Show(new AppointmentsView());
                    break;

                case UserRole.User:
                    AddNav("Мои записи", (s, e) => Show(new MyAppointmentsView()));
                    AddNav("Услуги", (s, e) => Show(new ServicesView()));
                    Show(new MyAppointmentsView());
                    break;
            }
        }

        private void AddNav(string text, RoutedEventHandler handler)
        {
            var btn = new Button
            {
                Content = text,
                Style = (Style)FindResource("NavButton")
            };
            btn.Click += handler;
            NavPanel.Children.Add(btn);
        }

        private void Show(UserControl view) => MainContent.Content = view;

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _data.CurrentUser = null;
            new LoginWindow().Show();
            Close();
        }
    }
}
