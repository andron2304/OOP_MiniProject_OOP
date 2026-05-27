namespace MusicPlayer.Domain;

/// <summary>
/// Представляє плейліст, який може містити треки та вкладені плейлисти (паттерн Composite).
/// </summary>
public class Playlist : IPlaylistComponent
{
    private readonly List<IPlaylistComponent> _components = [];

    public string Id { get; }
    public string Title { get; }
    public IReadOnlyList<IPlaylistComponent> Components => _components.AsReadOnly();

    public Playlist(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Назва плейліста не може бути пустою.", nameof(title));

        Id = Guid.NewGuid().ToString();
        Title = title;
    }

    public void AddComponent(IPlaylistComponent component)
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));

        _components.Add(component);
    }

    public void RemoveComponent(IPlaylistComponent component)
    {
        _components.Remove(component);
    }

    public int GetDuration() => _components.Sum(c => c.GetDuration());

    public int GetTrackCount()
    {
        int count = 0;
        foreach (var component in _components)
        {
            if (component is Track)
                count++;
            else if (component is Playlist playlist)
                count += playlist.GetTrackCount();
        }
        return count;
    }

    public override string ToString() => $"Плейліст: {Title} ({GetTrackCount()} треків, {GetDuration()}с)";
}
