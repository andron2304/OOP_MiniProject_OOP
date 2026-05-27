using System.Collections.Generic;

namespace MusicPlayer.Domain;

public class PlaylistState
{
    public List<Track> Playlist { get; set; } = new List<Track>();
    public int CurrentIndex { get; set; }
    public string StrategyName { get; set; } = "Normal";
    public int Version { get; set; } = 1;
}
