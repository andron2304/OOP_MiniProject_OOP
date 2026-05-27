using System.Threading.Tasks;

namespace MusicPlayer.Infrastructure;

public interface IDataStore
{
}

public interface IDataStore<T>
{
    Task SaveAsync(string filePath, T data);
    Task<T?> LoadAsync(string filePath);
}
