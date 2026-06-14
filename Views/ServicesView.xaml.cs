using System.Windows;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class ServicesView : UserControl
    {
        private readonly DataStore _data = DataStore.Instance;

        public ServicesView()
        {
            InitializeComponent();
            Grid.ItemsSource = _data.Services;

            // Пользователь (клиент) видит только каталог — без формы добавления/удаления
            if (_data.CurrentUser?.Role == UserRole.User)
            {
                FormPanel.Visibility = Visibility.Collapsed;
                FormColumn.Width = new GridLength(0);
                HeaderText.Text = "Каталог услуг";
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                !int.TryParse(DurationBox.Text, out int duration) ||
                !decimal.TryParse(PriceBox.Text, out decimal price))
            {
                MessageBox.Show("Проверьте поля: название, длительность и цена (числом).",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _data.Services.Add(new Service
            {
                Id = _data.NextServiceId(),
                Name = NameBox.Text.Trim(),
                Category = CategoryBox.Text.Trim(),
                DurationMinutes = duration,
                Price = price
            });

            NameBox.Clear(); CategoryBox.Clear(); DurationBox.Clear(); PriceBox.Clear();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Service s)
                _data.Services.Remove(s);
            else
                MessageBox.Show("Выберите услугу.");
        }
    }
}
