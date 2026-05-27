using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayer.Domain;

public class ShuffleStrategy : IPlaybackStrategy
{
    private readonly Random _rng;

    public ShuffleStrategy() : this(new Random()) { }
    public ShuffleStrategy(Random rng) => _rng = rng ?? new Random();

    public List<Track> Queue(List<Track> tracks)
    {
        if (tracks == null) return new List<Track>();
        return tracks.OrderBy(_ => _rng.Next()).ToList();
    }

    public void Reset(List<Track> tracks)
    {
        // No-op; reseeding or stateful shuffle could be supported here
    }
}
