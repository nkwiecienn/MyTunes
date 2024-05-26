using System.ComponentModel.DataAnnotations;

namespace MyTunes.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public ICollection<Song> ?Songs { get; set; }
    }
}