using System;
using System.Windows;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class AppointmentsView : UserControl
    {
        private readonly DataStore _data = DataStore.Instance;

        public AppointmentsView()
        {
            InitializeComponent();

            AppointmentsGrid.ItemsSource = _data.Appointments;
            ClientBox.ItemsSource = _data.Clients;
            ServiceBox.ItemsSource = _data.Services;
            MasterBox.ItemsSource = _data.Masters;

            DateBox.SelectedDate = DateTime.Today;

            for (int h = 9; h <= 19; h++)
            {
                TimeBox.Items.Add($"{h:00}:00");
                TimeBox.Items.Add($"{h:00}:30");
            }
            if (TimeBox.Items.Count > 0) TimeBox.SelectedIndex = 0;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (ClientBox.SelectedItem is not Client client ||
                ServiceBox.SelectedItem is not Service service ||
                MasterBox.SelectedItem is not Master master ||
                DateBox.SelectedDate is not DateTime date ||
                TimeBox.SelectedItem is not string time)
            {
                MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _data.Appointments.Add(new Appointment
            {
                Id = _data.NextAppointmentId(),
                Client = client,
                Service = service,
                Master = master,
                Date = date,
                TimeSlot = time,
                Status = "Запланирована"
            });
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsGrid.SelectedItem is Appointment appt)
                _data.Appointments.Remove(appt);
            else
                MessageBox.Show("Выберите запись в таблице.");
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsGrid.SelectedItem is Appointment appt)
            {
                appt.Status = "Завершена";
                AppointmentsGrid.Items.Refresh();
            }
            else
                MessageBox.Show("Выберите запись в таблице.");
        }
    }
}
