namespace MyTunes.ViewModels
{
    public class AddSongToPlaylistViewModel
    {
        public int SongId { get; set; }
        public int SelectedPlaylistId { get; set; }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Playlists { get; set; }
    }
}
