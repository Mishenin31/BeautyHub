using System.Windows;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class MastersView : UserControl
    {
        private readonly DataStore _data = DataStore.Instance;

        public MastersView()
        {
            InitializeComponent();
            Grid.ItemsSource = _data.Masters;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите ФИО мастера.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _data.Masters.Add(new Master
            {
                Id = _data.NextMasterId(),
                FullName = NameBox.Text.Trim(),
                Specialization = SpecBox.Text.Trim(),
                Phone = PhoneBox.Text.Trim()
            });

            NameBox.Clear(); SpecBox.Clear(); PhoneBox.Clear();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is Master m)
                _data.Masters.Remove(m);
            else
                MessageBox.Show("Выберите мастера.");
        }
    }
}
