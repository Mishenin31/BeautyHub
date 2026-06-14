using System;
using System.Collections.ObjectModel;

namespace BeautyHub.Models
{
    public enum UserRole
    {
        Admin,      // Администратор — полный доступ
        Employee,   // Сотрудник — управление записями, услугами, мастерами, клиентами
        User        // Пользователь (клиент) — свои записи и онлайн-запись
    }

    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";   // Демо: хранится в открытом виде. В реальном проекте — хеш!
        public string FullName { get; set; } = "";
        public UserRole Role { get; set; }

        // Для роли User — ссылка на карточку клиента
        public Client? Client { get; set; }

        public string RoleDisplay => Role switch
        {
            UserRole.Admin => "Администратор",
            UserRole.Employee => "Сотрудник",
            UserRole.User => "Пользователь",
            _ => Role.ToString()
        };
    }

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string Category { get; set; } = "";

        public string Display => $"{Name} ({DurationMinutes} мин) — {Price:N0} ₽";

        public override string ToString() => Name;
    }

    public class Master
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Specialization { get; set; } = "";
        public string Phone { get; set; } = "";

        public override string ToString() => $"{FullName} — {Specialization}";
    }

    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";

        public override string ToString() => $"{FullName} ({Phone})";
    }

    public class Appointment
    {
        public int Id { get; set; }
        public Client? Client { get; set; }
        public Master? Master { get; set; }
        public Service? Service { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; } = "";
        public string Status { get; set; } = "Запланирована";

        public string ClientName => Client?.FullName ?? "";
        public string MasterName => Master?.FullName ?? "";
        public string ServiceName => Service?.Name ?? "";
        public decimal Price => Service?.Price ?? 0;
        public string DateDisplay => Date.ToString("dd.MM.yyyy");
    }
}
