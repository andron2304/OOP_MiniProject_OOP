# MusicPlayer_OOP - Лаб 35 (Ітерація 2)

Консольний додаток музичного плеєра на .NET 8, який демонструє Clean Architecture та паттерни проектування.

## Архітектура

**Clean Architecture з 5 рівнів:**

- **Domain** (`MusicPlayer.Domain`) - Основні сутності (Track, Playlist) та інтерфейси (IPlaylistComponent, IPlaybackStrategy)
- **Application** (`MusicPlayer.Application`) - Бізнес-логіка (PlayerService)
- **Infrastructure** (`MusicPlayer.Infrastructure`) - Доступ до даних (IRepository<T>, InMemoryRepository<T>, JsonDataStore<T>, PlaylistState)
- **Console** (`MusicPlayer.Console`) - Інтерактивний CLI та обробка введення користувача
- **Tests** (`MusicPlayer.Tests`) - Набір тестів xUnit (12 тестів) для логіки Domain та Application

## Основні можливості (Ітерація 2)

- ✓ Моделі Domain Track та Playlist з 5 бізнес-правилами
- ✓ Паттерн Composite для вкладених структур плейлистів
- ✓ **Паттерн Strategy** для відтворення (NormalStrategy, ShuffleStrategy)
- ✓ **JSON-засноване сховище** (JsonDataStore<T>) з асинхронним доступом
- ✓ **Стан плейліста** (PlaylistState) для збереження конфігурації
- ✓ In-memory сховище з універсальними операціями CRUD
- ✓ Сервіс PlayerService на рівні Application
- ✓ Інтерактивне меню консолі
- ✓ Валідація конструктора та обробка винятків
- ✓ **Комплексні тести з паттерном AAA** (12 тестів xUnit)
- ✓ Аналітичні команди LINQ для запитів

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

### Комплексне тестове покриття (12 тестів xUnit)

**Домен інваріанти (4 тести):**
- Track конструктор з коректними даними
- Track конструктор з нульовою тривалістю (виняток)
- Track конструктор з пустою назвою (виняток)
- Playlist максимальна ємність (100 треків, виняток)

**Бізнес-правила (5 тестів):**
1. Заборона дублювання ID треків у плейлісті
2. Розрахунок тривалості вкладених плейлистів (рекурсивний)
3. Підрахунок треків з вкладеними плейлистами (рекурсивний)
4. Видалення компонента зменшує кількість треків
5. Відтворення пустого плейліста викидає виняток

**JSON серіалізація (2 тести):**
- SaveAsync зберігає PlaylistState до файлу
- LoadAsync десеріалізує PlaylistState з файлу

**Стратегії відтворення (1 тест):**
- NormalStrategy та ShuffleStrategy маніпулюють порядком треків

## Структура проекту

```
MusicPlayer/
├── src/
│   ├── MusicPlayer.Domain/
│   │   ├── IPlaylistComponent.cs
│   │   ├── IPlaybackStrategy.cs
│   │   ├── Track.cs
│   │   ├── Playlist.cs
│   │   ├── PlaylistState.cs
│   │   ├── NormalStrategy.cs
│   │   └── ShuffleStrategy.cs
│   ├── MusicPlayer.Application/
│   │   └── PlayerService.cs
│   ├── MusicPlayer.Infrastructure/
│   │   ├── IRepository.cs
│   │   ├── IDataStore.cs
│   │   ├── InMemoryRepository.cs
│   │   └── JsonDataStore.cs
│   └── MusicPlayer.Console/
│       └── Program.cs
├── tests/
│   └── MusicPlayer.Tests/
│       └── DomainTests.cs (12 тестів xUnit)
├── docs/
│   ├── vision.md
│   ├── backlog.md
│   ├── iteration-1.md
│   ├── iteration-2-plan.md
│   ├── iteration-2.md
│   ├── class-diagram.md
│   └── sequence-diagram.md
├── .github/workflows/
│   └── dotnet.yml
├── .gitignore
├── README.md
└── MusicPlayer.sln
```

## JSON сховище та збереження стану

**JsonDataStore<T>** - асинхронне, типізоване сховище для JSON-серіалізації:

```csharp
// Ініціалізація
var dataStore = new JsonDataStore<PlaylistState>(new JsonSerializerOptions { WriteIndented = true });

// Збереження стану плейліста
await dataStore.SaveAsync("playlist_state.json", playlistState);

// Завантаження стану
var loadedState = await dataStore.LoadAsync("playlist_state.json");
```

**PlaylistState** - зберігає конфігурацію відтворення:
- `Playlist: List<Track>` - список треків
- `CurrentIndex: int` - поточна позиція
- `StrategyName: string` - назва стратегії ("Normal" або "Shuffle")
- `Version: int` - версія стану

## Паттерн Strategy для відтворення

**IPlaybackStrategy** - інтерфейс для реалізації різних стратегій відтворення:

```csharp
public interface IPlaybackStrategy
{
    List<Track> Queue(List<Track> tracks);
    void Reset(List<Track> tracks);
}
```

### Реалізовані стратегії:

**NormalStrategy** - послідовне відтворення:
```csharp
var strategy = new NormalStrategy();
var queue = strategy.Queue(tracks); // Зберігає оригінальний порядок
```

**ShuffleStrategy** - випадкове перемішування:
```csharp
var strategy = new ShuffleStrategy();
var queue = strategy.Queue(tracks); // Перемішує порядок випадково
```

## Аналітичні LINQ команди

### Приклади запитів для аналізу плейлістів:

```csharp
var allTracks = playerService.GetAllTracks();

// 1. Треки довше за 300 секунд
var longTracks = allTracks.Where(t => t.Duration > 300).ToList();

// 2. Сортування за тривалістю (зростаючо)
var sortedByDuration = allTracks.OrderBy(t => t.Duration).ToList();

// 3. Групування за виконавцем та підрахунок
var tracksByArtist = allTracks
    .GroupBy(t => t.Artist)
    .Select(g => new { Artist = g.Key, Count = g.Count(), TotalDuration = g.Sum(t => t.Duration) })
    .ToList();

// 4. Найдовші треки (TOP 5)
var topLongestTracks = allTracks
    .OrderByDescending(t => t.Duration)
    .Take(5)
    .ToList();

// 5. Середня тривалість треків
var averageDuration = allTracks.Average(t => t.Duration);

// 6. Пошук по назві (регістронезалежний)
var searchResults = allTracks
    .Where(t => t.Title.Contains("Love", StringComparison.OrdinalIgnoreCase))
    .ToList();

// 7. Статистика за виконавцем
var artistStats = allTracks
    .GroupBy(t => t.Artist)
    .OrderByDescending(g => g.Count())
    .Select(g => new { Artist = g.Key, TrackCount = g.Count() })
    .ToList();
```

## Наступні кроки (Ітерація 3+)

- Сервіс управління чергою з підтримкою策略
- Розширені фільтри пошуку за метаданими
- Отримання дизайну з теми
- Інтеграція Entity Framework Core
- REST API для віддаленого доступу

## Ліцензія

Навчальний проект для демонстрації OOP.
