/* ============================================================
   Beauty Hub — скрипт базы данных для Microsoft SQL Server
   Создаёт БД, таблицы, связи и заполняет тестовыми данными.
   Запускать в SQL Server Management Studio (SSMS) или sqlcmd.
   ============================================================ */

-- 1. Создание базы данных --------------------------------------------------
IF DB_ID(N'BeautyHub') IS NULL
BEGIN
    CREATE DATABASE BeautyHub;
END
GO

USE BeautyHub;
GO

-- 2. Удаление таблиц при повторном запуске (в порядке зависимостей) ---------
IF OBJECT_ID(N'dbo.Appointments', N'U') IS NOT NULL DROP TABLE dbo.Appointments;
IF OBJECT_ID(N'dbo.Users',        N'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.Services',     N'U') IS NOT NULL DROP TABLE dbo.Services;
IF OBJECT_ID(N'dbo.Masters',      N'U') IS NOT NULL DROP TABLE dbo.Masters;
IF OBJECT_ID(N'dbo.Clients',      N'U') IS NOT NULL DROP TABLE dbo.Clients;
GO

-- 3. Таблица клиентов -------------------------------------------------------
CREATE TABLE dbo.Clients
(
    Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clients PRIMARY KEY,
    FullName  NVARCHAR(150)     NOT NULL,
    Phone     NVARCHAR(30)      NULL
);
GO

-- 4. Таблица мастеров -------------------------------------------------------
CREATE TABLE dbo.Masters
(
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Masters PRIMARY KEY,
    FullName        NVARCHAR(150)     NOT NULL,
    Specialization  NVARCHAR(150)     NULL,
    Phone           NVARCHAR(30)      NULL
);
GO

-- 5. Таблица услуг ----------------------------------------------------------
CREATE TABLE dbo.Services
(
    Id               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Services PRIMARY KEY,
    Name             NVARCHAR(150)     NOT NULL,
    Price            DECIMAL(10,2)     NOT NULL CONSTRAINT DF_Services_Price DEFAULT (0),
    DurationMinutes  INT               NOT NULL CONSTRAINT DF_Services_Duration DEFAULT (60),
    Category         NVARCHAR(100)     NULL
);
GO

-- 6. Таблица пользователей --------------------------------------------------
--    Role: 0 = Администратор, 1 = Сотрудник, 2 = Пользователь
--    ClientId заполняется только для роли "Пользователь" (роль 2)
CREATE TABLE dbo.Users
(
    Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Login     NVARCHAR(50)      NOT NULL,
    Password  NVARCHAR(255)     NOT NULL,   -- в проде хранить ХЕШ, а не открытый пароль
    FullName  NVARCHAR(150)     NOT NULL,
    Role      INT               NOT NULL CONSTRAINT DF_Users_Role DEFAULT (2),
    ClientId  INT               NULL,
    CONSTRAINT UQ_Users_Login UNIQUE (Login),
    CONSTRAINT CK_Users_Role CHECK (Role IN (0, 1, 2)),
    CONSTRAINT FK_Users_Clients FOREIGN KEY (ClientId)
        REFERENCES dbo.Clients (Id) ON DELETE SET NULL
);
GO

-- 7. Таблица записей --------------------------------------------------------
--    Status: 'Запланирована' | 'Завершена' | 'Отменена'
CREATE TABLE dbo.Appointments
(
    Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Appointments PRIMARY KEY,
    ClientId   INT               NOT NULL,
    MasterId   INT               NOT NULL,
    ServiceId  INT               NOT NULL,
    [Date]     DATE              NOT NULL,
    TimeSlot   NVARCHAR(10)      NOT NULL,
    Status     NVARCHAR(30)      NOT NULL CONSTRAINT DF_Appointments_Status DEFAULT (N'Запланирована'),
    CONSTRAINT FK_Appointments_Clients  FOREIGN KEY (ClientId)  REFERENCES dbo.Clients  (Id),
    CONSTRAINT FK_Appointments_Masters  FOREIGN KEY (MasterId)  REFERENCES dbo.Masters  (Id),
    CONSTRAINT FK_Appointments_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services (Id)
);
GO

-- Индексы для частых выборок (по клиенту и по дате)
CREATE INDEX IX_Appointments_ClientId ON dbo.Appointments (ClientId);
CREATE INDEX IX_Appointments_Date     ON dbo.Appointments ([Date]);
GO

/* ============================================================
   ЗАПОЛНЕНИЕ ТЕСТОВЫМИ ДАННЫМИ
   ============================================================ */

-- Услуги --------------------------------------------------------------------
INSERT INTO dbo.Services (Name, Price, DurationMinutes, Category) VALUES
    (N'Женская стрижка',       1500, 60,  N'Парикмахер'),
    (N'Мужская стрижка',       1000, 45,  N'Парикмахер'),
    (N'Окрашивание',           4000, 120, N'Парикмахер'),
    (N'Маникюр классический',  1200, 60,  N'Ногтевой сервис'),
    (N'Маникюр + гель-лак',    2000, 90,  N'Ногтевой сервис'),
    (N'Педикюр',               1800, 75,  N'Ногтевой сервис'),
    (N'Чистка лица',           2500, 80,  N'Косметология'),
    (N'Массаж лица',           2200, 50,  N'Косметология');
GO

-- Мастера -------------------------------------------------------------------
INSERT INTO dbo.Masters (FullName, Specialization, Phone) VALUES
    (N'Анна Соколова',    N'Парикмахер-стилист',  N'+7 900 111-22-33'),
    (N'Мария Иванова',    N'Мастер маникюра',     N'+7 900 222-33-44'),
    (N'Елена Петрова',    N'Косметолог',          N'+7 900 333-44-55'),
    (N'Ольга Кузнецова',  N'Парикмахер-колорист', N'+7 900 444-55-66');
GO

-- Клиенты -------------------------------------------------------------------
INSERT INTO dbo.Clients (FullName, Phone) VALUES
    (N'Дарья Смирнова',   N'+7 911 100-10-01'),
    (N'Виктория Попова',  N'+7 911 200-20-02'),
    (N'Алексей Морозов',  N'+7 911 300-30-03');
GO

-- Пользователи --------------------------------------------------------------
--   admin / admin       — администратор (роль 0)
--   employee / employee — сотрудник (роль 1)
--   user / user         — пользователь (роль 2), привязан к Дарье Смирновой
DECLARE @DaryaId INT = (SELECT Id FROM dbo.Clients WHERE FullName = N'Дарья Смирнова');

INSERT INTO dbo.Users (Login, Password, FullName, Role, ClientId) VALUES
    (N'admin',    N'admin',    N'Администратор', 0, NULL),
    (N'employee', N'employee', N'Анна Соколова',  1, NULL),
    (N'user',     N'user',     N'Дарья Смирнова', 2, @DaryaId);
GO

-- Записи --------------------------------------------------------------------
DECLARE @c1 INT = (SELECT Id FROM dbo.Clients  WHERE FullName = N'Дарья Смирнова');
DECLARE @c2 INT = (SELECT Id FROM dbo.Clients  WHERE FullName = N'Виктория Попова');
DECLARE @m1 INT = (SELECT Id FROM dbo.Masters  WHERE FullName = N'Анна Соколова');
DECLARE @m2 INT = (SELECT Id FROM dbo.Masters  WHERE FullName = N'Мария Иванова');
DECLARE @s1 INT = (SELECT Id FROM dbo.Services WHERE Name = N'Женская стрижка');
DECLARE @s2 INT = (SELECT Id FROM dbo.Services WHERE Name = N'Маникюр + гель-лак');

INSERT INTO dbo.Appointments (ClientId, MasterId, ServiceId, [Date], TimeSlot, Status) VALUES
    (@c1, @m1, @s1, CAST(GETDATE() AS DATE), N'10:00', N'Запланирована'),
    (@c2, @m2, @s2, CAST(GETDATE() AS DATE), N'12:30', N'Завершена');
GO

/* ============================================================
   ПРОВЕРОЧНЫЕ ЗАПРОСЫ (можно выполнить отдельно)
   ============================================================ */

-- Записи с расшифровкой связанных данных
SELECT  a.Id,
        a.[Date],
        a.TimeSlot,
        c.FullName  AS Клиент,
        s.Name      AS Услуга,
        m.FullName  AS Мастер,
        s.Price     AS Цена,
        a.Status
FROM    dbo.Appointments a
JOIN    dbo.Clients  c ON c.Id = a.ClientId
JOIN    dbo.Services s ON s.Id = a.ServiceId
JOIN    dbo.Masters  m ON m.Id = a.MasterId
ORDER BY a.[Date], a.TimeSlot;
GO

-- Выручка по завершённым записям
SELECT SUM(s.Price) AS Выручка
FROM   dbo.Appointments a
JOIN   dbo.Services s ON s.Id = a.ServiceId
WHERE  a.Status = N'Завершена';
GO
