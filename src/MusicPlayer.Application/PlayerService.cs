using MusicPlayer.Domain;
using MusicPlayer.Infrastructure;
using MusicPlayer.Infrastructure.Repositories;
using System.Linq;

namespace MusicPlayer.Application.Services;

/// <summary>
/// Сервіс Application, який координує операції плеєра та бізнес-логіку.
/// </summary>
public class PlayerService
{
    private readonly IRepository<Track> _trackRepository;
    private readonly IRepository<Playlist> _playlistRepository;
    private readonly IDataStore<PlaylistState> _dataStore;
    private IPlaybackStrategy _strategy = new NormalStrategy();

    public PlayerService(IRepository<Track> trackRepository, IRepository<Playlist> playlistRepository, IDataStore<PlaylistState>? dataStore = null)
    {
        _trackRepository = trackRepository ?? throw new ArgumentNullException(nameof(trackRepository));
        _playlistRepository = playlistRepository ?? throw new ArgumentNullException(nameof(playlistRepository));
        _dataStore = dataStore ?? new JsonDataStore<PlaylistState>();
    }

    /// <summary>
    /// Додає новий трек до сховища.
    /// </summary>
    public Track AddTrack(string title, string artist, int duration)
    {
        var track = new Track(title, artist, duration);
        _trackRepository.Add(track);
        return track;
    }

    /// <summary>
    /// Отримує всі треки зі сховища.
    /// </summary>
    public IEnumerable<Track> GetAllTracks()
    {
        return _trackRepository.GetAll();
    }

    /// <summary>
    /// Отримує трек за його ID.
    /// </summary>
    public Track? GetTrackById(string id)
    {
        return _trackRepository.GetById(id);
    }

    /// <summary>
    /// Створює новий плейліст.
    /// </summary>
    public Playlist CreatePlaylist(string name)
    {
        var playlist = new Playlist(name);
        _playlistRepository.Add(playlist);
        return playlist;
    }

    /// <summary>
    /// Додає трек до існуючого плейліста.
    /// Enforces business rules defined in Domain playlist.
    /// </summary>
    public void AddTrackToPlaylist(string playlistId, Track track)
    {
        var playlist = _playlistRepository.GetById(playlistId);
        if (playlist == null)
            throw new KeyNotFoundException($"Плейліст з ідентифікатором {playlistId} не знайдений.");

        playlist.AddComponent(track);
        _playlistRepository.Update(playlist);
    }

    /// <summary>
    /// Імітує відтворення треку та повертає статус.
    /// </summary>
    public string PlayTrack(Track track)
    {
        if (track == null)
            throw new ArgumentNullException(nameof(track));

        return $"♫ Зараз відтворюється: {track.Title} від {track.Artist} ({track.Duration}с)";
    }

    /// <summary>
    /// Імітує відтворення плейліста.
    /// Prevents playing empty playlists.
    /// </summary>
    public string PlayPlaylist(Playlist playlist)
    {
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));

        if (playlist.GetTrackCount() == 0)
            throw new InvalidOperationException("Плейліст порожній. Додайте треки перед відтворенням.");

        return $"▶ Відтворюється плейліст '{playlist.Title}' з {playlist.GetTrackCount()} треків (Всього: {playlist.GetDuration()}с)";
    }

    /// <summary>
    /// Configure playback strategy at runtime.
    /// </summary>
    public void SetStrategy(IPlaybackStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public void SetStrategyByName(string name)
    {
        if (string.Equals(name, "Shuffle", StringComparison.OrdinalIgnoreCase))
            _strategy = new ShuffleStrategy();
        else
            _strategy = new NormalStrategy();
    }

    /// <summary>
    /// Play using current strategy (returns textual simulation of the queue).
    /// </summary>
    public string PlayWithStrategy(Playlist playlist)
    {
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));

        var tracks = playlist.GetFlattenedTracks();
        if (!tracks.Any())
            throw new InvalidOperationException("Плейліст порожній. Немає чого відтворювати.");

        var queue = _strategy.Queue(tracks);
        return $"Queue ({_strategy.GetType().Name}): {string.Join(" -> ", queue.Select(t => t.Title))}";
    }

    /// <summary>
    /// Persistence helpers using IDataStore<PlaylistState>
    /// </summary>
    public async Task SavePlaylistAsync(Playlist playlist, string filePath)
    {
        if (playlist == null) throw new ArgumentNullException(nameof(playlist));

        var state = new PlaylistState
        {
            Playlist = playlist.GetFlattenedTracks(),
            CurrentIndex = 0,
            StrategyName = _strategy.GetType().Name,
            Version = 1
        };

        await _dataStore.SaveAsync(filePath, state).ConfigureAwait(false);
    }

    public async Task<PlaylistState?> LoadPlaylistStateAsync(string filePath)
    {
        return await _dataStore.LoadAsync(filePath).ConfigureAwait(false);
    }

    /// <summary>
    /// LINQ & Analytics
    /// </summary>
    public IEnumerable<Track> FilterByArtist(string artist)
    {
        return _trackRepository.GetAll().Where(t => string.Equals(t.Artist, artist, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Track> SortByDuration(bool ascending = true)
    {
        return ascending ? _trackRepository.GetAll().OrderBy(t => t.Duration) : _trackRepository.GetAll().OrderByDescending(t => t.Duration);
    }

    public int GetTotalDuration()
    {
        return _trackRepository.GetAll().Sum(t => t.Duration);
    }

    public IEnumerable<IGrouping<string, Track>> GroupByArtist()
    {
        return _trackRepository.GetAll().GroupBy(t => t.Artist);
    }
}
