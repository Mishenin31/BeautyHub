using System.Windows;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class ClientsView : UserControl
    {
        private readonly DataStore _data = DataStore.Instance;

        public ClientsView()
        {
            InitializeComponent();
            Grid.ItemsSource = _data.Clients;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите ФИО клиента.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _data.Clients.Add(new Client
            {
                Id = _data.NextClientId(),
                FullName = NameBox.Text.Trim(),
                Phone = PhoneBox.Text.Trim()
            });

            NameBox.Clear(); PhoneBox.Clear();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Client c)
                _data.Clients.Remove(c);
            else
                MessageBox.Show("Выберите клиента.");
        }
    }
}
