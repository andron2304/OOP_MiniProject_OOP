using MusicPlayer.Domain;
using MusicPlayer.Application.Services;
using MusicPlayer.Infrastructure.Repositories;

// Ініціалізація сховищ та сервісу
var trackRepository = new InMemoryRepository<Track>();
var playlistRepository = new InMemoryRepository<Playlist>();
var playerService = new PlayerService(trackRepository, playlistRepository);

// Попередня заповнення прикладами даних
var track1 = playerService.AddTrack("Богемська рапсодія", "Queen", 354);
var track2 = playerService.AddTrack("Уяви", "John Lennon", 183);
var track3 = playerService.AddTrack("Сходи до неба", "Led Zeppelin", 482);

var playlist = playerService.CreatePlaylist("Мої улюблені");
playerService.AddTrackToPlaylist(playlist.Id, track1);
playerService.AddTrackToPlaylist(playlist.Id, track2);

bool running = true;

while (running)
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║      Музичний плеєр - Ітерація 1    ║");
    Console.WriteLine("╚══════════════════════════════════════╝\n");
    Console.WriteLine("1. Переглянути все треки");
    Console.WriteLine("2. Додати новий трек");
    Console.WriteLine("3. Переглянути все плейлисти");
    Console.WriteLine("4. Створити плейліст");
    Console.WriteLine("5. Додати трек до плейліста");
    Console.WriteLine("6. Відтворити трек");
    Console.WriteLine("7. Відтворити плейліст");
    Console.WriteLine("8. Queries & Settings");
    Console.WriteLine("9. Вихід");
    Console.Write("\nВиберіть опцію: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                ViewAllTracks(playerService);
                break;

            case "2":
                AddNewTrack(playerService);
                break;

            case "3":
                ViewAllPlaylists(playlistRepository);
                break;

            case "4":
                CreatePlaylist(playerService);
                break;

            case "5":
                AddTrackToPlaylist(playerService, playlistRepository);
                break;

            case "6":
                PlayTrack(playerService);
                break;

            case "7":
                PlayPlaylist(playlistRepository, playerService);
                break;

            case "8":
                QueriesAndSettings(playerService, playlistRepository);
                break;

            case "9":
                running = false;
                Console.WriteLine("\nДо побачення!");
                break;

            default:
                Console.WriteLine("Невірна опція. Натисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n✗ Помилка: {ex.Message}");
        Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }
}

static void QueriesAndSettings(PlayerService service, IRepository<Playlist> playlistRepository)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("QUERIES & SETTINGS:\n");
    Console.WriteLine("1. Filter tracks by artist");
    Console.WriteLine("2. Sort tracks by duration");
    Console.WriteLine("3. Show aggregated stats (total duration)");
    Console.WriteLine("4. Group tracks by artist");
    Console.WriteLine("5. Save playlist to JSON");
    Console.WriteLine("6. Load playlist from JSON");
    Console.WriteLine("7. Set playback strategy (Normal/Shuffle)");
    Console.WriteLine("8. Back");
    Console.Write("Choose: ");

    var opt = Console.ReadLine();
    switch (opt)
    {
        case "1":
            Console.Write("Artist name: ");
            var artist = Console.ReadLine();
            var byArtist = service.FilterByArtist(artist ?? string.Empty).ToList();
            Console.WriteLine($"Found {byArtist.Count} tracks:");
            foreach (var t in byArtist) Console.WriteLine($"  - {t}");
            break;

        case "2":
            var sorted = service.SortByDuration().ToList();
            foreach (var t in sorted) Console.WriteLine($"  - {t} | {t.Duration}s");
            break;

        case "3":
            Console.WriteLine($"Total duration: {service.GetTotalDuration()}s");
            break;

        case "4":
            var groups = service.GroupByArtist();
            foreach (var g in groups)
            {
                Console.WriteLine($"{g.Key} ({g.Count()}):");
                foreach (var t in g) Console.WriteLine($"  - {t}");
            }
            break;

        case "5":
            var playlists = playlistRepository.GetAll().ToList();
            if (!playlists.Any()) { Console.WriteLine("No playlists to save."); break; }
            Console.Write("Choose playlist number to save: ");
            if (!int.TryParse(Console.ReadLine(), out int pi) || pi < 1 || pi > playlists.Count) { Console.WriteLine("Invalid"); break; }
            var p = playlists[pi - 1];
            Console.Write("File path (e.g. playlist.json): ");
            var path = Console.ReadLine() ?? "playlist.json";
            service.SavePlaylistAsync(p, path).GetAwaiter().GetResult();
            Console.WriteLine("Saved.");
            break;

        case "6":
            Console.Write("File path to load (e.g. playlist.json): ");
            var loadPath = Console.ReadLine() ?? "playlist.json";
            var state = service.LoadPlaylistStateAsync(loadPath).GetAwaiter().GetResult();
            if (state == null) Console.WriteLine("Load failed or file missing.");
            else Console.WriteLine($"Loaded {state.Playlist.Count} tracks, strategy {state.StrategyName}");
            break;

        case "7":
            Console.Write("Strategy (Normal/Shuffle): ");
            var s = Console.ReadLine();
            service.SetStrategyByName(s ?? "Normal");
            Console.WriteLine("Strategy set.");
            break;

        case "8":
        default:
            break;
    }

    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

static void ViewAllTracks(PlayerService service)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("ВСІ ТРЕКИ:\n");

    var tracks = service.GetAllTracks().ToList();
    if (!tracks.Any())
    {
        Console.WriteLine("Треки не знайдені.");
    }
    else
    {
        foreach (var track in tracks)
        {
            Console.WriteLine($"  • {track.Title} - {track.Artist} ({track.Duration}с) [ID: {track.Id[..8]}...]");
        }
    }

    Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
    Console.ReadKey();
}

static void AddNewTrack(PlayerService service)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("ДОДАТИ НОВИЙ ТРЕК:\n");

    Console.Write("Назва: ");
    string? title = Console.ReadLine();

    Console.Write("Виконавець: ");
    string? artist = Console.ReadLine();

    Console.Write("Тривалість (секунди): ");
    if (!int.TryParse(Console.ReadLine(), out int duration))
    {
        throw new ArgumentException("Тривалість повинна бути допустимим числом.");
    }

    var track = service.AddTrack(title!, artist!, duration);
    Console.WriteLine($"\n✓ Трек додано: {track}");
    Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
    Console.ReadKey();
}

static void ViewAllPlaylists(IRepository<Playlist> repository)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("ВСІ ПЛЕЙЛИСТИ:\n");

    var playlists = repository.GetAll().ToList();
    if (!playlists.Any())
    {
        Console.WriteLine("Плейлисти не знайдені.");
    }
    else
    {
        foreach (var playlist in playlists)
        {
            Console.WriteLine($"  ♪ {playlist} [ID: {playlist.Id[..8]}...]");
            foreach (var component in playlist.Components)
            {
                Console.WriteLine($"    ├─ {component.Title}");
            }
        }
    }

    Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
    Console.ReadKey();
}

static void CreatePlaylist(PlayerService service)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("СТВОРИТИ ПЛЕЙЛІСТ:\n");

    Console.Write("Назва плейліста: ");
    string? name = Console.ReadLine();

    var playlist = service.CreatePlaylist(name!);
    Console.WriteLine($"\n✓ Плейліст створено: {playlist}");
    Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
    Console.ReadKey();
}

static void AddTrackToPlaylist(PlayerService service, IRepository<Playlist> playlistRepository)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("ДОДАТИ ТРЕК ДО ПЛЕЙЛІСТА:\n");

    var playlists = playlistRepository.GetAll().ToList();
    if (!playlists.Any())
    {
        Console.WriteLine("Доступні плейлисти відсутні.");
        Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
        return;
    }

    for (int i = 0; i < playlists.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {playlists[i].Title}");
    }

    Console.Write("\nОберіть плейліст: ");
    if (!int.TryParse(Console.ReadLine(), out int playlistIndex) || playlistIndex < 1 || playlistIndex > playlists.Count)
    {
        throw new ArgumentException("Невірний вибір плейліста.");
    }

    var selectedPlaylist = playlists[playlistIndex - 1];
    var tracks = service.GetAllTracks().ToList();

    if (!tracks.Any())
    {
        Console.WriteLine("Доступні треки відсутні.");
        Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
        return;
    }

    for (int i = 0; i < tracks.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {tracks[i]}");
    }

    Console.Write("\nОберіть трек: ");
    if (!int.TryParse(Console.ReadLine(), out int trackIndex) || trackIndex < 1 || trackIndex > tracks.Count)
    {
        throw new ArgumentException("Невірний вибір треку.");
    }

    var selectedTrack = tracks[trackIndex - 1];
    service.AddTrackToPlaylist(selectedPlaylist.Id, selectedTrack);

    Console.WriteLine($"\n✓ Трек додано до плейліста '{selectedPlaylist.Title}'");
    Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
    Console.ReadKey();
}

static void PlayTrack(PlayerService service)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("ВІДТВОРИТИ ТРЕК:\n");

    var tracks = service.GetAllTracks().ToList();
    if (!tracks.Any())
    {
        Console.WriteLine("Доступні треки відсутні.");
        Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
        return;
    }

    for (int i = 0; i < tracks.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {tracks[i]}");
    }

    Console.Write("\nОберіть трек: ");
    if (!int.TryParse(Console.ReadLine(), out int trackIndex) || trackIndex < 1 || trackIndex > tracks.Count)
    {
        throw new ArgumentException("Невірний вибір треку.");
    }

    var result = service.PlayTrack(tracks[trackIndex - 1]);
    Console.WriteLine($"\n{result}");
    Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
    Console.ReadKey();
}

static void PlayPlaylist(IRepository<Playlist> repository, PlayerService service)
{
    Console.Clear();
    Console.WriteLine("═══════════════════════════════════\n");
    Console.WriteLine("ВІДТВОРИТИ ПЛЕЙЛІСТ:\n");

    var playlists = repository.GetAll().ToList();
    if (!playlists.Any())
    {
        Console.WriteLine("Доступні плейлисти відсутні.");
        Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
        return;
    }

    for (int i = 0; i < playlists.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {playlists[i]}");
    }

    Console.Write("\nОберіть плейліст: ");
    if (!int.TryParse(Console.ReadLine(), out int playlistIndex) || playlistIndex < 1 || playlistIndex > playlists.Count)
    {
        throw new ArgumentException("Невірний вибір плейліста.");
    }

    var result = service.PlayPlaylist(playlists[playlistIndex - 1]);
    Console.WriteLine($"\n{result}");
    Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
    Console.ReadKey();
}
