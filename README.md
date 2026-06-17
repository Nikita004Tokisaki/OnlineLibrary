-- =====================================================
-- БАЗА ДАННЫХ: OnlineLibrary
-- ОНЛАЙН БИБЛИОТЕКА
-- =====================================================

-- -----------------------------------------------------
-- Таблица 1: Users (пользователи)
-- -----------------------------------------------------
CREATE TABLE Users (
    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
    Login TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    DisplayName TEXT NOT NULL,
    Role TEXT NOT NULL DEFAULT 'user',
    Status TEXT NOT NULL DEFAULT 'active',
    FreezeReason TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Индексы для Users
CREATE INDEX idx_Users_Login ON Users(Login);
CREATE INDEX idx_Users_Role ON Users(Role);
CREATE INDEX idx_Users_Status ON Users(Status);
CREATE INDEX idx_Users_Email ON Users(Email);

-- -----------------------------------------------------
-- Таблица 2: Genres (жанры)
-- -----------------------------------------------------
CREATE TABLE Genres (
    GenreId INTEGER PRIMARY KEY AUTOINCREMENT,
    GenreName TEXT NOT NULL UNIQUE
);

-- -----------------------------------------------------
-- Таблица 3: Books (книги)
-- -----------------------------------------------------
CREATE TABLE Books (
    BookId INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    AuthorId INTEGER NOT NULL,
    Description TEXT,
    CoverPath TEXT,
    ContentPath TEXT NOT NULL,
    AvgRating REAL DEFAULT 0,
    Status TEXT NOT NULL DEFAULT 'active',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (AuthorId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Индексы для Books
CREATE INDEX idx_Books_Title ON Books(Title);
CREATE INDEX idx_Books_AuthorId ON Books(AuthorId);
CREATE INDEX idx_Books_Status ON Books(Status);
CREATE INDEX idx_Books_AvgRating ON Books(AvgRating);

-- -----------------------------------------------------
-- Таблица 4: BookGenres (связь книг и жанров)
-- -----------------------------------------------------
CREATE TABLE BookGenres (
    BookId INTEGER NOT NULL,
    GenreId INTEGER NOT NULL,
    PRIMARY KEY (BookId, GenreId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId) ON DELETE CASCADE
);

-- -----------------------------------------------------
-- Таблица 5: Reviews (отзывы)
-- -----------------------------------------------------
CREATE TABLE Reviews (
    ReviewId INTEGER PRIMARY KEY AUTOINCREMENT,
    BookId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    Rating INTEGER NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Content TEXT NOT NULL,
    Status TEXT NOT NULL DEFAULT 'active',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    UNIQUE(BookId, UserId)
);

-- Индексы для Reviews
CREATE INDEX idx_Reviews_BookId ON Reviews(BookId);
CREATE INDEX idx_Reviews_UserId ON Reviews(UserId);
CREATE INDEX idx_Reviews_Status ON Reviews(Status);
CREATE INDEX idx_Reviews_Rating ON Reviews(Rating);

-- -----------------------------------------------------
-- Таблица 6: UserBooks (списки книг пользователя)
-- -----------------------------------------------------
CREATE TABLE UserBooks (
    UserBookId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    BookId INTEGER NOT NULL,
    ListType TEXT NOT NULL CHECK (ListType IN ('planned', 'reading', 'completed', 'abandoned')),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
    UNIQUE(UserId, BookId)
);

-- Индексы для UserBooks
CREATE INDEX idx_UserBooks_UserId ON UserBooks(UserId);
CREATE INDEX idx_UserBooks_BookId ON UserBooks(BookId);
CREATE INDEX idx_UserBooks_ListType ON UserBooks(ListType);

-- -----------------------------------------------------
-- Таблица 7: Complaints (жалобы)
-- -----------------------------------------------------
CREATE TABLE Complaints (
    ComplaintId INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL CHECK (Type IN ('book', 'author', 'review')),
    TargetId INTEGER NOT NULL,
    ComplainantId INTEGER NOT NULL,
    Reason TEXT NOT NULL,
    Status TEXT NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    ProcessedAt DATETIME,
    AdminId INTEGER,
    FOREIGN KEY (ComplainantId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (AdminId) REFERENCES Users(UserId) ON DELETE SET NULL
);

-- Индексы для Complaints
CREATE INDEX idx_Complaints_Status ON Complaints(Status);
CREATE INDEX idx_Complaints_Type_Target ON Complaints(Type, TargetId);
CREATE INDEX idx_Complaints_ComplainantId ON Complaints(ComplainantId);

-- -----------------------------------------------------
-- Таблица 8: AuthorRequests (заявки на роль автора)
-- -----------------------------------------------------
CREATE TABLE AuthorRequests (
    RequestId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Reason TEXT NOT NULL,
    Status TEXT NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    ProcessedAt DATETIME,
    AdminId INTEGER,
    AdminComment TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (AdminId) REFERENCES Users(UserId) ON DELETE SET NULL
);

-- Индексы для AuthorRequests
CREATE INDEX idx_AuthorRequests_Status ON AuthorRequests(Status);
CREATE INDEX idx_AuthorRequests_UserId ON AuthorRequests(UserId);

-- -----------------------------------------------------
-- Таблица 9: UnfreezeRequests (заявки на снятие заморозки)
-- -----------------------------------------------------
CREATE TABLE UnfreezeRequests (
    RequestId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    AppealText TEXT NOT NULL,
    Status TEXT NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    ProcessedAt DATETIME,
    AdminId INTEGER,
    AdminComment TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (AdminId) REFERENCES Users(UserId) ON DELETE SET NULL
);

-- Индексы для UnfreezeRequests
CREATE INDEX idx_UnfreezeRequests_Status ON UnfreezeRequests(Status);
CREATE INDEX idx_UnfreezeRequests_UserId ON UnfreezeRequests(UserId);

-- -----------------------------------------------------
-- Таблица 10: BookFreezeAppeals (оспаривание заморозки книги)
-- -----------------------------------------------------
CREATE TABLE BookFreezeAppeals (
    AppealId INTEGER PRIMARY KEY AUTOINCREMENT,
    BookId INTEGER NOT NULL,
    AuthorId INTEGER NOT NULL,
    AppealText TEXT NOT NULL,
    Status TEXT NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    ProcessedAt DATETIME,
    AdminId INTEGER,
    AdminComment TEXT,
    FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
    FOREIGN KEY (AuthorId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (AdminId) REFERENCES Users(UserId) ON DELETE SET NULL
);

-- Индексы для BookFreezeAppeals
CREATE INDEX idx_BookFreezeAppeals_Status ON BookFreezeAppeals(Status);
CREATE INDEX idx_BookFreezeAppeals_BookId ON BookFreezeAppeals(BookId);

-- =====================================================
-- ЗАПОЛНЕНИЕ НАЧАЛЬНЫМИ ДАННЫМИ
-- =====================================================

-- Жанры
INSERT INTO Genres (GenreName) VALUES 
('Роман'),
('Детектив'),
('Фантастика'),
('Фэнтези'),
('Научная литература'),
('Поэзия'),
('Драма'),
('Комедия'),
('Триллер'),
('Ужасы'),
('Историческая проза'),
('Биография'),
('Психология'),
('Философия');

-- Пользователи (пароли в реальном приложении хешируются)
INSERT INTO Users (Login, PasswordHash, Email, DisplayName, Role, Status) VALUES
('admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'admin@onlinelibrary.com', 'Главный администратор', 'admin', 'active'),
('ivanwriter', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'ivan@writer.com', 'Иван Петров', 'author', 'active'),
('mariawriter', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'maria@writer.com', 'Мария Сидорова', 'author', 'active'),
('reader1', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'reader1@mail.com', 'Алексей Читатель', 'user', 'active'),
('reader2', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'reader2@mail.com', 'Елена Книголюб', 'user', 'active'),
('frozen_user', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'frozen@mail.com', 'Заблокированный', 'user', 'frozen', 'Нарушение правил: спам в комментариях');

-- Книги
INSERT INTO Books (Title, AuthorId, Description, CoverPath, ContentPath, AvgRating) VALUES
('Тени прошлого', 2, 'Мистический роман о тайнах семьи...', '/covers/teni_proshlogo.jpg', '/books/teni_proshlogo.txt', 4.5),
('Путь к звездам', 2, 'Научно-фантастический роман о космических путешествиях...', '/covers/put_k_zvezdam.jpg', '/books/put_k_zvezdam.txt', 4.8),
('Детектив из провинции', 3, 'Захватывающий детектив о расследовании в маленьком городке...', '/covers/detektiv.jpg', '/books/detektiv.txt', 4.2),
('Философия жизни', 3, 'Размышления о смысле жизни...', '/covers/filosofiya.jpg', '/books/filosofiya.txt', 4.0),
('Война миров', 2, 'Классическая фантастика о вторжении марсиан...', '/covers/war_of_worlds.jpg', '/books/war_of_worlds.txt', 4.9);

-- Связи книг с жанрами
INSERT INTO BookGenres (BookId, GenreId) VALUES
(1, 1), (1, 4), (1, 7),      -- Тени прошлого: Роман, Фэнтези, Драма
(2, 3), (2, 5),               -- Путь к звездам: Фантастика, Научная литература
(3, 2), (3, 1),               -- Детектив из провинции: Детектив, Роман
(4, 13), (4, 14),             -- Философия жизни: Психология, Философия
(5, 3), (5, 9);               -- Война миров: Фантастика, Триллер

-- Отзывы
INSERT INTO Reviews (BookId, UserId, Rating, Content) VALUES
(1, 4, 5, 'Очень захватывающая книга! Прочитал на одном дыхании.'),
(1, 5, 4, 'Хороший сюжет, но концовка немного предсказуема.'),
(2, 4, 5, 'Лучшая фантастика года! Рекомендую всем.'),
(2, 5, 5, 'Потрясающе! Жду продолжения.'),
(3, 4, 4, 'Интересный детектив, держит в напряжении.'),
(5, 4, 5, 'Классика, которую нужно прочитать каждому.');

-- Списки книг пользователей
INSERT INTO UserBooks (UserId, BookId, ListType) VALUES
(4, 1, 'reading'),      -- Алексей читает "Тени прошлого"
(4, 2, 'planned'),      -- Алексей планирует "Путь к звездам"
(4, 5, 'completed'),    -- Алексей прочитал "Война миров"
(5, 1, 'completed'),    -- Елена прочитала "Тени прошлого"
(5, 3, 'reading'),      -- Елена читает "Детектив из провинции"
(5, 4, 'planned');      -- Елена планирует "Философию жизни"

-- Жалобы
INSERT INTO Complaints (Type, TargetId, ComplainantId, Reason, Status) VALUES
('review', 1, 5, 'Содержит оскорбления', 'pending'),
('book', 3, 4, 'Неверно указан автор', 'pending'),
('author', 2, 5, 'Плагиат', 'rejected');

-- Заявки на роль автора
INSERT INTO AuthorRequests (UserId, Reason, Status) VALUES
(4, 'Написал несколько рассказов, хочу публиковать книги в библиотеке', 'pending'),
(5, 'Профессиональный писатель, есть опубликованные произведения', 'accepted');

-- Заявки на снятие заморозки
INSERT INTO UnfreezeRequests (UserId, AppealText, Status) VALUES
(6, 'Я не нарушал правила, прошу разморозить аккаунт', 'pending');

-- =====================================================
-- КОНЕЦ СКРИПТА
-- =====================================================
