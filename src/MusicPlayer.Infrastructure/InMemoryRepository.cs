namespace MusicPlayer.Infrastructure.Repositories;

/// <summary>
/// Реалізація in-memory сховища з використанням Dictionary для зберігання.
/// </summary>
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<string, T> _storage = [];

    public T? GetById(string id)
    {
        return _storage.TryGetValue(id, out var entity) ? entity : null;
    }

    public IEnumerable<T> GetAll()
    {
        return _storage.Values.ToList();
    }

    public void Add(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var id = entity.GetType().GetProperty("Id")?.GetValue(entity)?.ToString();
        if (string.IsNullOrEmpty(id))
            throw new InvalidOperationException("Сутність повинна мати властивість Id.");

        _storage[id] = entity;
    }

    public void Update(T entity)
    {
        var id = entity?.GetType().GetProperty("Id")?.GetValue(entity)?.ToString();
        if (string.IsNullOrEmpty(id))
            throw new InvalidOperationException("Сутність повинна мати властивість Id.");

        if (!_storage.ContainsKey(id))
            throw new KeyNotFoundException($"Сутність з ідентифікатором {id} не знайдена.");

        _storage[id] = entity!;
    }

    public void Delete(string id)
    {
        _storage.Remove(id);
    }
}
