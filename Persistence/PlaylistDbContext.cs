using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class PlaylistDbContext
    {
        public PlaylistDbContext() 
        { 
            Playlists = new List<Playlist>();
        }
        public List<Playlist> Playlists { get; set; }
    }
}
