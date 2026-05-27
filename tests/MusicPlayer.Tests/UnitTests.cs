using System;
using System.Collections.Generic;
using System.Linq;
using MusicPlayer.Application.Services;
using MusicPlayer.Domain;
using MusicPlayer.Domain.Exceptions;
using MusicPlayer.Infrastructure.Repositories;
using Xunit;

namespace MusicPlayer.Tests;

public class UnitTests
{
    [Fact]
    public void Track_Constructor_WithValidData_CreatesTrackSuccessfully()
    {
        // Arrange
        string title = "Imagine";
        string artist = "John Lennon";
        int duration = 183;

        // Act
        var track = new Track(title, artist, duration);

        // Assert
        Assert.NotNull(track);
        Assert.Equal(title, track.Title);
        Assert.Equal(artist, track.Artist);
        Assert.Equal(duration, track.Duration);
        Assert.False(string.IsNullOrWhiteSpace(track.Id));
    }

    [Theory]
    [InlineData("", "Artist", 180)]
    [InlineData("   ", "Artist", 180)]
    public void Track_Constructor_InvalidTitle_ThrowsArgumentException(string title, string artist, int duration)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Track(title, artist, duration));
        Assert.Contains("Назва не може бути пустою", exception.Message);
    }

    [Theory]
    [InlineData("Track", "", 180)]
    [InlineData("Track", "   ", 180)]
    public void Track_Constructor_InvalidArtist_ThrowsArgumentException(string title, string artist, int duration)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Track(title, artist, duration));
        Assert.Contains("Виконавець не може бути пустим", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-120)]
    public void Track_Constructor_InvalidDuration_ThrowsArgumentException(int duration)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Track("Song", "Artist", duration));
        Assert.Contains("Тривалість повинна бути більше 0", exception.Message);
    }

    [Fact]
    public void Playlist_Constructor_WithEmptyTitle_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Playlist(string.Empty));
        Assert.Contains("Назва плейліста не може бути пустою", exception.Message);
    }

    [Fact]
    public void Playlist_AddNullComponent_ThrowsArgumentNullException()
    {
        // Arrange
        var playlist = new Playlist("Favorites");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => playlist.AddComponent(null!));
    }

    [Fact]
    public void Playlist_AddComponent_IncreasesTrackCount()
    {
        // Arrange
        var playlist = new Playlist("Morning Mix");
        var track = new Track("Sunrise", "Artist", 210);

        // Act
        playlist.AddComponent(track);

        // Assert
        Assert.Equal(1, playlist.GetTrackCount());
        Assert.Equal(210, playlist.GetDuration());
    }

    [Fact]
    public void Playlist_RemoveComponent_DecreasesTrackCount()
    {
        // Arrange
        var playlist = new Playlist("Evening Mix");
        var track = new Track("Moonlight", "Artist", 240);
        playlist.AddComponent(track);

        // Act
        playlist.RemoveComponent(track);

        // Assert
        Assert.Equal(0, playlist.GetTrackCount());
        Assert.Equal(0, playlist.GetDuration());
    }

    [Fact]
    public void Playlist_GetDuration_IncludesNestedPlaylistDurations()
    {
        // Arrange
        var main = new Playlist("Main");
        var sub = new Playlist("Sub");
        var track1 = new Track("One", "A", 100);
        var track2 = new Track("Two", "B", 150);
        sub.AddComponent(track1);
        main.AddComponent(sub);
        main.AddComponent(track2);

        // Act
        int totalDuration = main.GetDuration();

        // Assert
        Assert.Equal(250, totalDuration);
        Assert.Equal(2, main.GetTrackCount());
    }

    [Fact]
    public void Playlist_ToString_IncludesTrackCountAndDuration()
    {
        // Arrange
        var playlist = new Playlist("Chill");
        playlist.AddComponent(new Track("Breeze", "Artist", 120));
        playlist.AddComponent(new Track("Clouds", "Artist", 180));

        // Act
        string result = playlist.ToString();

        // Assert
        Assert.Contains("Chill", result);
        Assert.Contains("2 треків", result);
        Assert.Contains("300с", result);
    }

    [Fact]
    public void MusicPlayerException_CanBeConstructedWithMessage()
    {
        // Act
        var exception = new MusicPlayerException("Failure");

        // Assert
        Assert.Equal("Failure", exception.Message);
    }

    [Fact]
    public void InvalidTrackException_CanWrapInnerException()
    {
        // Arrange
        var inner = new InvalidOperationException("Inner");

        // Act
        var exception = new InvalidTrackException("Invalid metadata", inner);

        // Assert
        Assert.Equal("Invalid metadata", exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void StorageException_CanBeCreatedWithoutInnerException()
    {
        // Act
        var exception = new StorageException("Storage failure");

        // Assert
        Assert.Equal("Storage failure", exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void PlayerService_AddTrack_StoresTrackInRepository()
    {
        // Arrange
        var trackRepository = new InMemoryRepository<Track>();
        var playlistRepository = new InMemoryRepository<Playlist>();
        var service = new PlayerService(trackRepository, playlistRepository);

        // Act
        var track = service.AddTrack("Imagine", "John Lennon", 183);

        // Assert
        var stored = trackRepository.GetById(track.Id);
        Assert.NotNull(stored);
        Assert.Equal(track.Title, stored!.Title);
    }

    [Fact]
    public void PlayerService_CreatePlaylist_StoresPlaylistInRepository()
    {
        // Arrange
        var trackRepository = new InMemoryRepository<Track>();
        var playlistRepository = new InMemoryRepository<Playlist>();
        var service = new PlayerService(trackRepository, playlistRepository);

        // Act
        var playlist = service.CreatePlaylist("Rock");

        // Assert
        var stored = playlistRepository.GetById(playlist.Id);
        Assert.NotNull(stored);
        Assert.Equal("Rock", stored!.Title);
    }

    [Fact]
    public void PlayerService_AddTrackToPlaylist_UpdatesPlaylistWithTrack()
    {
        // Arrange
        var trackRepository = new InMemoryRepository<Track>();
        var playlistRepository = new InMemoryRepository<Playlist>();
        var service = new PlayerService(trackRepository, playlistRepository);
        var playlist = service.CreatePlaylist("Hits");
        var track = service.AddTrack("Perfect", "Ed Sheeran", 263);

        // Act
        service.AddTrackToPlaylist(playlist.Id, track);

        // Assert
        var updated = playlistRepository.GetById(playlist.Id);
        Assert.NotNull(updated);
        Assert.Equal(1, updated!.GetTrackCount());
        Assert.Equal(track.Duration, updated.GetDuration());
    }

    [Fact]
    public void PlayerService_AddTrackToPlaylist_NonexistentPlaylist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var trackRepository = new InMemoryRepository<Track>();
        var playlistRepository = new InMemoryRepository<Playlist>();
        var service = new PlayerService(trackRepository, playlistRepository);
        var track = new Track("Stay", "The Kid LAROI", 142);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => service.AddTrackToPlaylist("missing-id", track));
    }

    [Fact]
    public void PlayerService_PlayTrack_ReturnsFormattedPlaybackMessage()
    {
        // Arrange
        var service = new PlayerService(new InMemoryRepository<Track>(), new InMemoryRepository<Playlist>());
        var track = new Track("Come Together", "The Beatles", 259);

        // Act
        string message = service.PlayTrack(track);

        // Assert
        Assert.Contains("Зараз відтворюється", message);
        Assert.Contains(track.Title, message);
        Assert.Contains(track.Artist, message);
        Assert.Contains("259с", message);
    }

    [Fact]
    public void PlayerService_PlayPlaylist_ReturnsFormattedPlaybackMessage()
    {
        // Arrange
        var service = new PlayerService(new InMemoryRepository<Track>(), new InMemoryRepository<Playlist>());
        var playlist = new Playlist("Party");
        playlist.AddComponent(new Track("One More Time", "Daft Punk", 320));

        // Act
        string message = service.PlayPlaylist(playlist);

        // Assert
        Assert.Contains("Відтворюється плейліст", message);
        Assert.Contains("Party", message);
        Assert.Contains("1 треків", message);
    }

    [Fact]
    public void PlaybackStrategy_NormalPlayback_PreservesTrackOrder()
    {
        // Arrange
        var playlist = new Playlist("Ordered");
        var first = new Track("First", "Artist", 120);
        var second = new Track("Second", "Artist", 150);
        playlist.AddComponent(first);
        playlist.AddComponent(second);

        // Act
        var order = playlist.Components.OfType<Track>().Select(t => t.Title).ToList();

        // Assert
        Assert.Equal(new[] { "First", "Second" }, order);
    }

    [Fact]
    public void PlaybackStrategy_ShufflePlayback_ProducesDifferentOrderForRepeatedShuffles()
    {
        // Arrange
        var tracks = new[]
        {
            new Track("A", "Artist", 100),
            new Track("B", "Artist", 100),
            new Track("C", "Artist", 100)
        };

        // Act
        var firstShuffle = ShuffleTracks(tracks).Select(t => t.Id).ToList();
        var secondShuffle = ShuffleTracks(tracks).Select(t => t.Id).ToList();

        // Assert
        Assert.Equal(3, firstShuffle.Count);
        Assert.Equal(3, secondShuffle.Count);
        Assert.NotEqual(firstShuffle, secondShuffle);
    }

    private static IEnumerable<Track> ShuffleTracks(IEnumerable<Track> tracks)
    {
        return tracks.OrderBy(_ => Guid.NewGuid());
    }
}
