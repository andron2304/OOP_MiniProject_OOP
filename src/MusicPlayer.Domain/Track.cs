namespace MusicPlayer.Domain;

/// <summary>
/// Представляє один трек в музичній бібліотеці.
/// </summary>
public class Track : IPlaylistComponent
{
    public string Id { get; }
    public string Title { get; }
    public string Artist { get; }
    public int Duration { get; } // в секундах

    public Track(string title, string artist, int duration)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Назва не може бути пустою.", nameof(title));
        
        if (string.IsNullOrWhiteSpace(artist))
            throw new ArgumentException("Виконавець не може бути пустим.", nameof(artist));
        
        if (duration <= 0)
            throw new ArgumentException("Тривалість повинна бути більше 0.", nameof(duration));

        Id = Guid.NewGuid().ToString();
        Title = title;
        Artist = artist;
        Duration = duration;
    }

    public int GetDuration() => Duration;

    public override string ToString() => $"{Title} - {Artist} ({Duration}с)";
}
