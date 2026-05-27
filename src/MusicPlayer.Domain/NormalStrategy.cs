using System.Collections.Generic;
using System.Linq;

namespace MusicPlayer.Domain;

public class NormalStrategy : IPlaybackStrategy
{
    public List<Track> Queue(List<Track> tracks)
    {
        // Preserve original order
        return tracks.ToList();
    }

    public void Reset(List<Track> tracks)
    {
        // No-op for normal ordering
    }
}
