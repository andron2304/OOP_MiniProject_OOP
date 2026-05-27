namespace MusicPlayer.Domain;

/// <summary>
/// Представляє плейліст, який може містити треки та вкладені плейлисти (паттерн Composite).
/// </summary>
public class Playlist : IPlaylistComponent
{
    private readonly List<IPlaylistComponent> _components = new List<IPlaylistComponent>();
    private const int MaxCapacity = 100;

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

        // Enforce capacity: only track components contribute to capacity
        int currentTrackCount = GetTrackCount();
        int addingTracks = component is Track ? 1 : (component is Playlist p ? p.GetTrackCount() : 0);
        if (currentTrackCount + addingTracks > MaxCapacity)
            throw new InvalidOperationException($"Плейліст не може містити більше {MaxCapacity} треків.");

        // Prevent duplicate track IDs when adding a Track
        if (component is Track t)
        {
            var existingIds = new HashSet<string>(GetFlattenedTracks().Select(x => x.Id));
            if (existingIds.Contains(t.Id))
                throw new InvalidOperationException("Трек з таким Id вже існує в плейлісті.");
        }

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

    public List<Track> GetFlattenedTracks()
    {
        var list = new List<Track>();
        foreach (var component in _components)
        {
            if (component is Track t)
                list.Add(t);
            else if (component is Playlist p)
                list.AddRange(p.GetFlattenedTracks());
        }
        return list;
    }

    public override string ToString() => $"Плейліст: {Title} ({GetTrackCount()} треків, {GetDuration()}с)";
}
