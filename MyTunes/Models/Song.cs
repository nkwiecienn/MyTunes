using System.ComponentModel.DataAnnotations;

namespace MyTunes.Models
{
    public class Song
    {
        [Key]
        public int SongId { get; set; }
        public string Name { get; set; }
        public Album Album { get; set; }
        public ICollection<User> ?Favorites { get; set; }
        public ICollection<Playlist> ?Playlists { get; set; }
    }
}