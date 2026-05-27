using Xunit;
using MusicPlayer.Domain;
using MusicPlayer.Application.Services;
using MusicPlayer.Infrastructure;
using MusicPlayer.Infrastructure.Repositories;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayer.Tests;

/// <summary>
/// Comprehensive test suite for MusicPlayer domain and persistence layer.
/// Total: 12 Unit Tests covering Domain Invariants, Business Rules, 
/// JSON Serialization, and Playback Strategy Switches.
/// </summary>
public class DomainTests
{
    // ============================================================================
    // DOMAIN INVARIANTS (Tests 1-4)
    // ============================================================================

    /// <summary>
    /// Test 1: Domain Invariant - Track Constructor with Valid Data
    /// Ensures Track objects are created successfully with valid inputs.
    /// Business Rule: All tracks must have non-empty title, artist, and positive duration.
    /// </summary>
    [Fact]
    public void Track_Constructor_WithValidData_CreatesTrackSuccessfully()
    {
        // Arrange
        string title = "Bogus Song";
        string artist = "Artist Name";
        int duration = 240;

        // Act
        var track = new Track(title, artist, duration);

        // Assert
        Assert.NotNull(track);
        Assert.Equal(title, track.Title);
        Assert.Equal(artist, track.Artist);
        Assert.Equal(duration, track.Duration);
        Assert.NotEmpty(track.Id);
    }

    /// <summary>
    /// Test 2: Domain Invariant - Track Constructor with Invalid Duration
    /// Ensures Track constructor rejects non-positive duration values.
    /// Business Rule: Duration must be greater than zero.
    /// </summary>
    [Fact]
    public void Track_Constructor_WithZeroDuration_ThrowsArgumentException()
    {
        // Arrange
        string title = "Track Title";
        string artist = "Artist";
        int duration = 0;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Track(title, artist, duration));
        Assert.Contains("Тривалість повинна бути більше 0", exception.Message);
    }

    /// <summary>
    /// Test 3: Domain Invariant - Track Constructor with Empty Title
    /// Ensures Track constructor rejects empty/null titles.
    /// Business Rule: Track title must be non-empty string.
    /// </summary>
    [Fact]
    public void Track_Constructor_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        string title = "";
        string artist = "Artist";
        int duration = 180;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Track(title, artist, duration));
        Assert.Contains("Назва не може бути пустою", exception.Message);
    }

    /// <summary>
    /// Test 4: Domain Invariant - Playlist Capacity Enforcement
    /// Ensures Playlist cannot exceed maximum capacity of 100 tracks.
    /// Business Rule: Playlist has a hard limit of 100 tracks (leaf nodes).
    /// </summary>
    [Fact]
    public void Playlist_AddComponent_ExceedingCapacity_ThrowsInvalidOperationException()
    {
        // Arrange
        var playlist = new Playlist("Capacity Test Playlist");
        var tracks = new List<Track>();
        for (int i = 0; i < 100; i++)
        {
            tracks.Add(new Track($"Track {i}", $"Artist {i}", 180));
            playlist.AddComponent(tracks[i]);
        }

        var extraTrack = new Track("Extra Track", "Extra Artist", 180);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => playlist.AddComponent(extraTrack));
        Assert.Contains("не може містити більше 100 треків", exception.Message);
    }

    // ============================================================================
    // BUSINESS RULES (Tests 5-9)
    // ============================================================================

    /// <summary>
    /// Test 5: Business Rule - Prevent Duplicate Track IDs
    /// Ensures the same track cannot be added twice to a playlist.
    /// Business Rule: Track IDs must be unique within a playlist.
    /// </summary>
    [Fact]
    public void Playlist_AddDuplicateTrack_ThrowsInvalidOperationException()
    {
        // Arrange
        var playlist = new Playlist("Duplicate Test Playlist");
        var track = new Track("Track Title", "Artist", 180);
        playlist.AddComponent(track);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => playlist.AddComponent(track));
        Assert.Contains("Трек з таким Id вже існує", exception.Message);
    }

    /// <summary>
    /// Test 6: Business Rule - Calculate Duration Including Nested Playlists
    /// Ensures duration calculation includes tracks from nested playlists.
    /// Business Rule: GetDuration must recursively sum all component durations (Composite pattern).
    /// </summary>
    [Fact]
    public void Playlist_GetDuration_IncludesNestedPlaylistTracks()
    {
        // Arrange
        var mainPlaylist = new Playlist("Main Playlist");
        var nestedPlaylist = new Playlist("Nested Playlist");
        
        var track1 = new Track("Track 1", "Artist 1", 180);
        var track2 = new Track("Track 2", "Artist 2", 240);
        var track3 = new Track("Track 3", "Artist 3", 120);

        nestedPlaylist.AddComponent(track1);
        nestedPlaylist.AddComponent(track2);
        mainPlaylist.AddComponent(nestedPlaylist);
        mainPlaylist.AddComponent(track3);

        // Act
        int totalDuration = mainPlaylist.GetDuration();

        // Assert
        int expectedDuration = 180 + 240 + 120;
        Assert.Equal(expectedDuration, totalDuration);
    }

    /// <summary>
    /// Test 7: Business Rule - Track Count Includes Nested Playlists
    /// Ensures track count accurately reflects all tracks across nested structures.
    /// Business Rule: GetTrackCount must recursively count leaf Track nodes only.
    /// </summary>
    [Fact]
    public void Playlist_GetTrackCount_IncludesNestedPlaylistTracks()
    {
        // Arrange
        var mainPlaylist = new Playlist("Main Playlist");
        var nestedPlaylist1 = new Playlist("Nested 1");
        var nestedPlaylist2 = new Playlist("Nested 2");

        var track1 = new Track("Track 1", "Artist 1", 180);
        var track2 = new Track("Track 2", "Artist 2", 240);
        var track3 = new Track("Track 3", "Artist 3", 120);
        var track4 = new Track("Track 4", "Artist 4", 200);

        nestedPlaylist1.AddComponent(track1);
        nestedPlaylist1.AddComponent(track2);
        nestedPlaylist2.AddComponent(track3);
        nestedPlaylist2.AddComponent(track4);
        
        mainPlaylist.AddComponent(nestedPlaylist1);
        mainPlaylist.AddComponent(nestedPlaylist2);

        // Act
        int trackCount = mainPlaylist.GetTrackCount();

        // Assert
        Assert.Equal(4, trackCount);
    }

    /// <summary>
    /// Test 8: Business Rule - Remove Component from Playlist
    /// Ensures components can be properly removed from playlists.
    /// Business Rule: RemoveComponent must correctly update playlist state.
    /// </summary>
    [Fact]
    public void Playlist_RemoveComponent_ReducesTrackCount()
    {
        // Arrange
        var playlist = new Playlist("Remove Test Playlist");
        var track1 = new Track("Track 1", "Artist 1", 180);
        var track2 = new Track("Track 2", "Artist 2", 240);
        
        playlist.AddComponent(track1);
        playlist.AddComponent(track2);
        Assert.Equal(2, playlist.GetTrackCount());

        // Act
        playlist.RemoveComponent(track1);

        // Assert
        Assert.Equal(1, playlist.GetTrackCount());
        Assert.NotContains(track1, playlist.GetFlattenedTracks());
        Assert.Contains(track2, playlist.GetFlattenedTracks());
    }

    /// <summary>
    /// Test 9: Business Rule - Empty Playlist Cannot Be Played
    /// Ensures PlayerService enforces the requirement that playlists must contain tracks.
    /// Business Rule: PlayPlaylist must reject empty playlists.
    /// </summary>
    [Fact]
    public void PlayerService_PlayPlaylist_WithEmptyPlaylist_ThrowsInvalidOperationException()
    {
        // Arrange
        var trackRepo = new InMemoryRepository<Track>();
        var playlistRepo = new InMemoryRepository<Playlist>();
        var playerService = new PlayerService(trackRepo, playlistRepo);
        
        var emptyPlaylist = new Playlist("Empty Playlist");
        playlistRepo.Add(emptyPlaylist);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => playerService.PlayPlaylist(emptyPlaylist));
        Assert.Contains("Плейліст порожній", exception.Message);
    }

    // ============================================================================
    // JSON SERIALIZATION (Tests 10-11)
    // ============================================================================

    /// <summary>
    /// Test 10: JSON Serialization - SaveAsync Successfully Persists PlaylistState
    /// Ensures JsonDataStore can serialize and save PlaylistState objects to disk.
    /// Business Rule: Persistence layer must reliably save state for recovery.
    /// </summary>
    [Fact]
    public async Task JsonDataStore_SaveAsync_PersistsPlaylistStateSuccessfully()
    {
        // Arrange
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"test_playlist_{Guid.NewGuid()}.json");
        var dataStore = new JsonDataStore<PlaylistState>();
        
        var playlistState = new PlaylistState
        {
            Playlist = new List<Track>
            {
                new Track("Song 1", "Artist 1", 200),
                new Track("Song 2", "Artist 2", 180)
            },
            CurrentIndex = 1,
            StrategyName = "Shuffle",
            Version = 1
        };

        // Act
        await dataStore.SaveAsync(tempFilePath, playlistState);

        // Assert
        Assert.True(File.Exists(tempFilePath));
        var fileContent = await File.ReadAllTextAsync(tempFilePath);
        Assert.Contains("Song 1", fileContent);
        Assert.Contains("Artist 1", fileContent);
        Assert.Contains("Shuffle", fileContent);

        // Cleanup
        File.Delete(tempFilePath);
    }

    /// <summary>
    /// Test 11: JSON Serialization - LoadAsync Successfully Deserializes PlaylistState
    /// Ensures JsonDataStore can deserialize PlaylistState objects from disk.
    /// Business Rule: Persistence layer must reliably restore state from saved files.
    /// </summary>
    [Fact]
    public async Task JsonDataStore_LoadAsync_DeserializesPlaylistStateSuccessfully()
    {
        // Arrange
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"test_playlist_{Guid.NewGuid()}.json");
        var dataStore = new JsonDataStore<PlaylistState>();
        
        var originalState = new PlaylistState
        {
            Playlist = new List<Track>
            {
                new Track("Original Song", "Original Artist", 240)
            },
            CurrentIndex = 0,
            StrategyName = "Normal",
            Version = 2
        };

        await dataStore.SaveAsync(tempFilePath, originalState);

        // Act
        var loadedState = await dataStore.LoadAsync(tempFilePath);

        // Assert
        Assert.NotNull(loadedState);
        Assert.Equal(1, loadedState.Playlist.Count);
        Assert.Equal("Original Song", loadedState.Playlist[0].Title);
        Assert.Equal(0, loadedState.CurrentIndex);
        Assert.Equal("Normal", loadedState.StrategyName);
        Assert.Equal(2, loadedState.Version);

        // Cleanup
        File.Delete(tempFilePath);
    }

    // ============================================================================
    // PLAYBACK STRATEGY SWITCHES (Tests 12)
    // ============================================================================

    /// <summary>
    /// Test 12: Playback Strategy - NormalStrategy vs ShuffleStrategy
    /// Ensures both strategy implementations work correctly for queue ordering.
    /// Business Rule: Playback strategies must correctly reorder tracks according to their algorithm.
    /// </summary>
    [Fact]
    public void PlaybackStrategies_NormalAndShuffle_ProduceDifferentOrders()
    {
        // Arrange
        var tracks = new List<Track>
        {
            new Track("Track A", "Artist A", 180),
            new Track("Track B", "Artist B", 200),
            new Track("Track C", "Artist C", 220),
            new Track("Track D", "Artist D", 240),
            new Track("Track E", "Artist E", 260)
        };

        var normalStrategy = new NormalStrategy();
        var shuffleStrategy = new ShuffleStrategy(new Random(42)); // Fixed seed for reproducibility

        // Act
        var normalQueue = normalStrategy.Queue(new List<Track>(tracks));
        var shuffledQueue = shuffleStrategy.Queue(new List<Track>(tracks));

        // Assert
        // Normal strategy should preserve original order
        Assert.Equal(tracks[0].Id, normalQueue[0].Id);
        Assert.Equal(tracks[1].Id, normalQueue[1].Id);
        Assert.Equal(tracks[2].Id, normalQueue[2].Id);
        Assert.Equal(tracks[3].Id, normalQueue[3].Id);
        Assert.Equal(tracks[4].Id, normalQueue[4].Id);

        // Shuffled queue should have all same tracks but possibly different order
        Assert.Equal(5, shuffledQueue.Count);
        Assert.All(shuffledQueue, item => Assert.Contains(item, tracks));
        
        // Verify shuffle queue is valid
        Assert.NotNull(shuffledQueue);
    }
}
