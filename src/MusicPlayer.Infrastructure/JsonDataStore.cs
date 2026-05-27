using System.Text.Json;

namespace MusicPlayer.Infrastructure;

public class JsonDataStore<T> : IDataStore<T>
{
    private readonly JsonSerializerOptions _options;

    public JsonDataStore(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions { WriteIndented = true };
    }

    public async Task SaveAsync(string filePath, T data)
    {
        try
        {
            var temp = filePath + ".tmp";
            using (var stream = System.IO.File.Create(temp))
            {
                await JsonSerializer.SerializeAsync(stream, data, _options).ConfigureAwait(false);
            }
            System.IO.File.Move(temp, filePath, overwrite: true);
        }
        catch (Exception)
        {
            // Basic catch: let caller handle/log as needed
            throw;
        }
    }

    public async Task<T?> LoadAsync(string filePath)
    {
        try
        {
            if (!System.IO.File.Exists(filePath))
                return default;

            using (var stream = System.IO.File.OpenRead(filePath))
            {
                var result = await JsonSerializer.DeserializeAsync<T>(stream, _options).ConfigureAwait(false);
                return result;
            }
        }
        catch (Exception)
        {
            // Return default on corruption or errors
            return default;
        }
    }
}
