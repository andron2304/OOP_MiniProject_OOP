namespace MusicPlayer.Infrastructure.Repositories;

/// <summary>
/// Універсальний інтерфейс сховища для операцій CRUD.
/// </summary>
public interface IRepository<T> where T : class
{
    T? GetById(string id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(string id);
}
