using System;
using System.Collections.ObjectModel;
using BeautyHub.Models;

namespace BeautyHub
{
    // Простое хранилище данных в памяти (singleton).
    // Для реального проекта замените на БД (EF Core / SQLite).
    public class DataStore
    {
        private static DataStore? _instance;
        public static DataStore Instance => _instance ??= new DataStore();

        public ObservableCollection<Service> Services { get; } = new();
        public ObservableCollection<Master> Masters { get; } = new();
        public ObservableCollection<Client> Clients { get; } = new();
        public ObservableCollection<Appointment> Appointments { get; } = new();
        public ObservableCollection<User> Users { get; } = new();

        // Текущий вошедший пользователь (сессия)
        public User? CurrentUser { get; set; }

        private int _serviceId = 1, _masterId = 1, _clientId = 1, _apptId = 1, _userId = 1;

        private DataStore()
        {
            Seed();
        }

        public int NextServiceId() => _serviceId++;
        public int NextMasterId() => _masterId++;
        public int NextClientId() => _clientId++;
        public int NextAppointmentId() => _apptId++;
        public int NextUserId() => _userId++;

        // Проверка логина/пароля. Возвращает пользователя или null.
        public User? Authenticate(string login, string password)
        {
            foreach (var u in Users)
                if (string.Equals(u.Login, login, StringComparison.OrdinalIgnoreCase) && u.Password == password)
                    return u;
            return null;
        }

        public bool LoginExists(string login)
        {
            foreach (var u in Users)
                if (string.Equals(u.Login, login, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        // Регистрация нового пользователя (всегда роль User + новая карточка клиента)
        public User RegisterUser(string login, string password, string fullName, string phone)
        {
            var client = new Client { Id = NextClientId(), FullName = fullName, Phone = phone };
            Clients.Add(client);

            var user = new User
            {
                Id = NextUserId(),
                Login = login,
                Password = password,
                FullName = fullName,
                Role = UserRole.User,
                Client = client
            };
            Users.Add(user);
            return user;
        }

        private void Seed()
        {
            Services.Add(new Service { Id = NextServiceId(), Name = "Женская стрижка", Price = 1500, DurationMinutes = 60, Category = "Парикмахер" });
            Services.Add(new Service { Id = NextServiceId(), Name = "Мужская стрижка", Price = 1000, DurationMinutes = 45, Category = "Парикмахер" });
            Services.Add(new Service { Id = NextServiceId(), Name = "Окрашивание", Price = 4000, DurationMinutes = 120, Category = "Парикмахер" });
            Services.Add(new Service { Id = NextServiceId(), Name = "Маникюр классический", Price = 1200, DurationMinutes = 60, Category = "Ногтевой сервис" });
            Services.Add(new Service { Id = NextServiceId(), Name = "Маникюр + гель-лак", Price = 2000, DurationMinutes = 90, Category = "Ногтевой сервис" });
            Services.Add(new Service { Id = NextServiceId(), Name = "Педикюр", Price = 1800, DurationMinutes = 75, Category = "Ногтевой сервис" });
            Services.Add(new Service { Id = NextServiceId(), Name = "Чистка лица", Price = 2500, DurationMinutes = 80, Category = "Косметология" });
            Services.Add(new Service { Id = NextServiceId(), Name = "Массаж лица", Price = 2200, DurationMinutes = 50, Category = "Косметология" });

            Masters.Add(new Master { Id = NextMasterId(), FullName = "Анна Соколова", Specialization = "Парикмахер-стилист", Phone = "+7 900 111-22-33" });
            Masters.Add(new Master { Id = NextMasterId(), FullName = "Мария Иванова", Specialization = "Мастер маникюра", Phone = "+7 900 222-33-44" });
            Masters.Add(new Master { Id = NextMasterId(), FullName = "Елена Петрова", Specialization = "Косметолог", Phone = "+7 900 333-44-55" });
            Masters.Add(new Master { Id = NextMasterId(), FullName = "Ольга Кузнецова", Specialization = "Парикмахер-колорист", Phone = "+7 900 444-55-66" });

            Clients.Add(new Client { Id = NextClientId(), FullName = "Дарья Смирнова", Phone = "+7 911 100-10-01" });
            Clients.Add(new Client { Id = NextClientId(), FullName = "Виктория Попова", Phone = "+7 911 200-20-02" });
            Clients.Add(new Client { Id = NextClientId(), FullName = "Алексей Морозов", Phone = "+7 911 300-30-03" });

            // Тестовые учётные записи (логин / пароль):
            //   admin / admin       — администратор
            //   employee / employee — сотрудник
            //   user / user         — пользователь (привязан к клиенту Дарья Смирнова)
            Users.Add(new User { Id = NextUserId(), Login = "admin", Password = "admin", FullName = "Администратор", Role = UserRole.Admin });
            Users.Add(new User { Id = NextUserId(), Login = "employee", Password = "employee", FullName = "Анна Соколова", Role = UserRole.Employee });
            Users.Add(new User { Id = NextUserId(), Login = "user", Password = "user", FullName = "Дарья Смирнова", Role = UserRole.User, Client = Clients[0] });

            Appointments.Add(new Appointment
            {
                Id = NextAppointmentId(),
                Client = Clients[0],
                Master = Masters[0],
                Service = Services[0],
                Date = DateTime.Today,
                TimeSlot = "10:00",
                Status = "Запланирована"
            });
            Appointments.Add(new Appointment
            {
                Id = NextAppointmentId(),
                Client = Clients[1],
                Master = Masters[1],
                Service = Services[4],
                Date = DateTime.Today,
                TimeSlot = "12:30",
                Status = "Завершена"
            });
        }
    }
}
