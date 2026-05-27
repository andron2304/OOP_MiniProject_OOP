namespace MusicPlayer.Domain;

/// <summary>
/// Інтерфейс паттерну Composite для компонентів плейліста (Трек або Плейліст).
/// </summary>
public interface IPlaylistComponent
{
    string Title { get; }
    int GetDuration();
}
