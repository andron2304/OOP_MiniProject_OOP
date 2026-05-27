using MusicPlayer.Domain;
using MusicPlayer.Infrastructure.Repositories;

namespace MusicPlayer.Application.Services;

/// <summary>
/// Сервіс Application, який координує операції плеєра та бізнес-логіку.
/// </summary>
public class PlayerService
{
    private readonly IRepository<Track> _trackRepository;
    private readonly IRepository<Playlist> _playlistRepository;

    public PlayerService(IRepository<Track> trackRepository, IRepository<Playlist> playlistRepository)
    {
        _trackRepository = trackRepository ?? throw new ArgumentNullException(nameof(trackRepository));
        _playlistRepository = playlistRepository ?? throw new ArgumentNullException(nameof(playlistRepository));
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
    /// </summary>
    public string PlayPlaylist(Playlist playlist)
    {
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));

        return $"▶ Відтворюється плейліст '{playlist.Title}' з {playlist.GetTrackCount()} треків (Всього: {playlist.GetDuration()}с)";
    }
}
