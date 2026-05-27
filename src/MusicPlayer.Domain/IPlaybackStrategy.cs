using System.Collections.Generic;

namespace MusicPlayer.Domain;

public interface IPlaybackStrategy
{
    List<Track> Queue(List<Track> tracks);
    void Reset(List<Track> tracks);
}
