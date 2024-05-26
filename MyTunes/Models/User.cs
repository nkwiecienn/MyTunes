using System.ComponentModel.DataAnnotations;

namespace MyTunes.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string ?Username { get; set; }
        public string ?Password { get; set; } 
        public ICollection<Playlist> ?Playlists { get; set; }
        public ICollection<Song> ?Favorites { get; set; }
    }
}