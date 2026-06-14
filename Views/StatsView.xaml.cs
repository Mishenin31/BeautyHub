using System.Linq;
using System.Windows.Controls;
using BeautyHub.Models;

namespace BeautyHub.Views
{
    public partial class StatsView : UserControl
    {
        private readonly DataStore _data = DataStore.Instance;

        public StatsView()
        {
            InitializeComponent();
            LoadStats();
        }

        private void LoadStats()
        {
            var appts = _data.Appointments;
            var completed = appts.Where(a => a.Status == "Завершена").ToList();

            TotalAppts.Text = appts.Count.ToString();
            Completed.Text = completed.Count.ToString();
            Revenue.Text = completed.Sum(a => a.Price).ToString("N0") + " ₽";
            People.Text = $"{_data.Clients.Count} / {_data.Masters.Count}";

            var grouped = appts
                .Where(a => a.Service != null)
                .GroupBy(a => a.Service!.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            int max = grouped.Count > 0 ? grouped.Max(x => x.Count) : 1;

            ServiceStats.ItemsSource = grouped
                .Select(x => new { x.Name, x.Count, Max = max })
                .ToList();
        }
    }
}
