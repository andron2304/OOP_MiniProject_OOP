using MusicPlayer.Domain;
using MusicPlayer.Application.Services;
using MusicPlayer.Infrastructure.Repositories;
using Xunit;

namespace MusicPlayer.Tests;

public class DomainTests
{
    // Тест 1: Конструктор Track з допустимими даними створює трек успішно
    [Fact]
    public void Track_Constructor_WithValidData_CreatesTrackSuccessfully()
    {
        // Arrange (Підготовка)
        string title = "Богемська рапсодія";
        string artist = "Queen";
        int duration = 354;

        // Act (Дія)
        var track = new Track(title, artist, duration);

        // Assert (Утвердження)
        Assert.NotNull(track);
        Assert.Equal(title, track.Title);
        Assert.Equal(artist, track.Artist);
        Assert.Equal(duration, track.Duration);
        Assert.NotEmpty(track.Id);
    }

    // Тест 2: Конструктор Track з нульовою тривалістю викидає ArgumentException
    [Fact]
    public void Track_Constructor_WithZeroDuration_ThrowsArgumentException()
    {
        // Arrange (Підготовка)
        string title = "Пісня";
        string artist = "Виконавець";
        int duration = 0;

        // Act & Assert (Дія та Утвердження)
        var exception = Assert.Throws<ArgumentException>(() => new Track(title, artist, duration));
        Assert.Contains("Тривалість повинна бути більше 0", exception.Message);
    }

    // Тест 3: Розрахунок тривалості плейліста сумує всі тривалості компонентів
    [Fact]
    public void Playlist_GetDuration_SumsAllComponentDurations()
    {
        // Arrange (Підготовка)
        var playlist = new Playlist("Тестовий плейліст");
        var track1 = new Track("Трек 1", "Виконавець 1", 180);
        var track2 = new Track("Трек 2", "Виконавець 2", 240);
        var track3 = new Track("Трек 3", "Виконавець 3", 200);

        playlist.AddComponent(track1);
        playlist.AddComponent(track2);
        playlist.AddComponent(track3);

        // Act (Дія)
        int totalDuration = playlist.GetDuration();

        // Assert (Утвердження)
        Assert.Equal(620, totalDuration);
    }

    // Тест 4: PlayerService AddTrack зберігає трек в сховище
    [Fact]
    public void PlayerService_AddTrack_StoresTrackInRepository()
    {
        // Arrange (Підготовка)
        var trackRepository = new InMemoryRepository<Track>();
        var playlistRepository = new InMemoryRepository<Playlist>();
        var service = new PlayerService(trackRepository, playlistRepository);

        // Act (Дія)
        var track = service.AddTrack("Уяви", "John Lennon", 183);

        // Assert (Утвердження)
        var retrievedTrack = trackRepository.GetById(track.Id);
        Assert.NotNull(retrievedTrack);
        Assert.Equal("Уяви", retrievedTrack.Title);
    }

    // Тест 5: PlayerService PlayTrack повертає очікуваний формат рядка
    [Fact]
    public void PlayerService_PlayTrack_ReturnsFormattedPlaybackMessage()
    {
        // Arrange (Підготовка)
        var trackRepository = new InMemoryRepository<Track>();
        var playlistRepository = new InMemoryRepository<Playlist>();
        var service = new PlayerService(trackRepository, playlistRepository);
        var track = new Track("Сходи до неба", "Led Zeppelin", 482);

        // Act (Дія)
        string result = service.PlayTrack(track);

        // Assert (Утвердження)
        Assert.Contains("Зараз відтворюється", result);
        Assert.Contains("Сходи до неба", result);
        Assert.Contains("Led Zeppelin", result);
        Assert.Contains("482с", result);
    }
}
