using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class MyAppointmentsView : UserControl
    {
        private readonly DataStore _data = DataStore.Instance;
        private readonly Client? _client;
        private readonly ObservableCollection<Appointment> _myAppts = new();

        public MyAppointmentsView()
        {
            InitializeComponent();

            _client = _data.CurrentUser?.Client;
            HeaderText.Text = _client != null ? $"Мои записи — {_client.FullName}" : "Мои записи";

            ServiceBox.ItemsSource = _data.Services;
            MasterBox.ItemsSource = _data.Masters;
            DateBox.SelectedDate = DateTime.Today;

            for (int h = 9; h <= 19; h++)
            {
                TimeBox.Items.Add($"{h:00}:00");
                TimeBox.Items.Add($"{h:00}:30");
            }
            TimeBox.SelectedIndex = 0;

            RefreshMyAppointments();
            Grid.ItemsSource = _myAppts;
        }

        // Фильтруем только записи текущего клиента
        private void RefreshMyAppointments()
        {
            _myAppts.Clear();
            if (_client == null) return;
            foreach (var a in _data.Appointments.Where(a => a.Client == _client))
                _myAppts.Add(a);
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            if (_client == null)
            {
                MessageBox.Show("Эта учётная запись не привязана к карточке клиента.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ServiceBox.SelectedItem is not Service service ||
                MasterBox.SelectedItem is not Master master ||
                DateBox.SelectedDate is not DateTime date ||
                TimeBox.SelectedItem is not string time)
            {
                MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var appt = new Appointment
            {
                Id = _data.NextAppointmentId(),
                Client = _client,
                Service = service,
                Master = master,
                Date = date,
                TimeSlot = time,
                Status = "Запланирована"
            };

            _data.Appointments.Add(appt);
            _myAppts.Add(appt);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is not Appointment appt)
            {
                MessageBox.Show("Выберите запись.");
                return;
            }

            _data.Appointments.Remove(appt);
            _myAppts.Remove(appt);
        }
    }
}
