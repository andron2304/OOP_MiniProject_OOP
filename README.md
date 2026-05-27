# MusicPlayer_OOP - Лаб 34 Ітерація 1

Консольний додаток музичного плеєра на .NET 8, який демонструє Clean Architecture та паттерни проектування.

## Архітектура

**Clean Architecture з 5 рівнів:**

- **Domain** (`MusicPlayer.Domain`) - Основні сутності (Track, Playlist) та інтерфейси (IPlaylistComponent)
- **Application** (`MusicPlayer.Application`) - Бізнес-логіка (PlayerService)
- **Infrastructure** (`MusicPlayer.Infrastructure`) - Доступ до даних (IRepository<T>, InMemoryRepository<T>)
- **Console** (`MusicPlayer.Console`) - Інтерактивний CLI та обробка введення користувача
- **Tests** (`MusicPlayer.Tests`) - Набір тестів xUnit для логіки Domain та Application

## Основні можливості (Ітерація 1)

- ✓ Моделі Domain Track та Playlist
- ✓ Паттерн Composite для вкладених структур плейлистів
- ✓ In-memory сховище з універсальними операціями CRUD
- ✓ Сервіс PlayerService на рівні Application
- ✓ Інтерактивне меню консолі
- ✓ Валідація конструктора та обробка винятків
- ✓ Модульні тести з паттерном AAA

## Передумови

- .NET 8 SDK або новіша версія
- Git (для клонування)

## Налаштування

1. **Клонуйте репозиторій** (або розпакуйте папку проекту)
   ```bash
   git clone <repository-url>
   cd MusicPlayer
   ```

2. **Відновіть залежності**
   ```bash
   dotnet restore
   ```

3. **Побудуйте рішення**
   ```bash
   dotnet build
   ```

## Запуск консольного додатка

```bash
dotnet run --project src/MusicPlayer.Console/MusicPlayer.Console.csproj
```

### Опції меню консолі
1. Переглянути всі треки
2. Додати новий трек
3. Переглянути всі плейлисти
4. Створити плейліст
5. Додати трек до плейліста
6. Відтворити трек
7. Відтворити плейліст
8. Вихід

## Запуск тестів

```bash
dotnet test
```

**Покриття тестами:**
- Валідація конструктора Track
- Обробка неправильної тривалості
- Розрахунок тривалості плейліста
- Збереження треку в сховищі PlayerService
- Форматування повідомлення про відтворення

## Структура проекту

```
MusicPlayer/
├── src/
│   ├── MusicPlayer.Domain/
│   │   ├── IPlaylistComponent.cs
│   │   ├── Track.cs
│   │   └── Playlist.cs
│   ├── MusicPlayer.Application/
│   │   └── PlayerService.cs
│   ├── MusicPlayer.Infrastructure/
│   │   ├── IRepository.cs
│   │   └── InMemoryRepository.cs
│   └── MusicPlayer.Console/
│       └── Program.cs
├── tests/
│   └── MusicPlayer.Tests/
│       └── DomainTests.cs
├── docs/
│   ├── vision.md
│   ├── backlog.md
│   ├── iteration-1.md
│   ├── class-diagram.md
│   └── sequence-diagram.md
├── .github/workflows/
│   └── dotnet.yml
├── .gitignore
├── README.md
└── MusicPlayer.sln
```

## Наступні кроки (Ітерація 2+)

- Збереження на основі файлів (JSON)
- Розширені алгоритми перемішування
- Сервіс управління чергою
- Відстеження історії відтворення
- Інтеграція Entity Framework Core

## Ліцензія

Навчальний проект для демонстрації OOP.
