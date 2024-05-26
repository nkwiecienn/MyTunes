using System.ComponentModel.DataAnnotations;

namespace MyTunes.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }
        public string Name { get; set; }

        [Display(Name = "DebutDate")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        
        public DateTime DebutDate { get; set; }
        public ICollection<Album> ?Albums { get; set; }
    }
}