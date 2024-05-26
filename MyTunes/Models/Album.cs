using System.ComponentModel.DataAnnotations;

namespace MyTunes.Models
{
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }
        public string Name { get; set; }

        [Display(Name = "ReleaseDate")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public Artist Artist { get; set; }
        public ICollection<Song> ?Songs { get; set; }
    }
}