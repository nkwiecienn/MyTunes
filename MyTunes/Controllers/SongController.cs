using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTunes.Data;
using MyTunes.Models;
using MyTunes.ViewModels;

namespace MyTunes.Controllers
{
    public class SongController : Controller
    {
        private readonly MyTunesContext _context;

        public SongController(MyTunesContext context)
        {
            _context = context;
        }

        public List<Song> Top(MyTunesContext _context)
        {
            var songsQuery = from s in _context.Song
                             .Include(s => s.Album)
                             .ThenInclude(a => a.Artist)
                             .Include(s => s.Favorites)
                             orderby s.Favorites.Count descending
                             select s;

            var songs = songsQuery.ToList();

            return songs;
        }

        public async Task<IActionResult> MostLiked()
        {
            var topSongs = Top(_context);

            ViewData["Username"] = HttpContext.Session.GetString("Username");
            return View(topSongs);
        }

        [Route("/Song/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Song
        public async Task<IActionResult> Index(int id) 
        {
            var album = await _context.Album.FindAsync(id);
            ViewBag.AlbumId = id;
            ViewBag.Album = album;

            var songs = await _context.Song.Where(s => s.Album == album).ToListAsync();

            ViewData["Username"] = HttpContext.Session.GetString("Username");

            return View(songs);
        }

        public async Task<IActionResult> PlaylistIndex(int playlistId)
        {
            var playlist = await _context.Playlist
                .Include(p => p.Songs)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            if (playlist == null)
            {
                return NotFound();
            }

            var recommendedSongs = Recomended(_context, playlistId);

            ViewBag.PlaylistId = playlistId;
            ViewBag.Recommended = recommendedSongs;
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            return View(playlist.Songs);
        }

        public List<Song> Recomended(MyTunesContext _context, int playlistId)
        {
            var mostPopularGenre = (from aa in _context.Album
                            join ss in _context.Song on aa.AlbumId equals ss.Album.AlbumId
                            join c in _context.Playlist on ss.SongId equals c.Songs.Select(s => s.SongId).FirstOrDefault()
                            where c.PlaylistId == playlistId
                            group aa by aa.Genre into g
                            orderby g.Count() descending
                            select g.Key).FirstOrDefault();

            var songsQuery = from s in _context.Song
                            join a in _context.Album on s.Album.AlbumId equals a.AlbumId
                            where a.Genre == mostPopularGenre
                            && !_context.Playlist.Any(p => p.PlaylistId == playlistId && p.Songs.Any(ps => ps.SongId == s.SongId))
                            orderby s.Favorites.Count descending
                            select s;

            var songs = songsQuery.Take(5).ToList();

            return songs;
        }

        // GET: Song/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Song/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Song/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,Name")] Song song, int id)
        {
            var album = _context.Album.Where(a => a.AlbumId == id).First();
            song.Album = album;

            _context.Add(song);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Song", new {id = album.AlbumId});
        }

        // GET: Song/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        // POST: Song/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SongId,Name")] Song song)
        {
            if (id != song.SongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.SongId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(song);
        }

        // GET: Song/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Song/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Song == null)
            {
                return Problem("Entity set 'MyTunesContext.Song'  is null.");
            }
            var song = await _context.Song.FindAsync(id);
            if (song != null)
            {
                _context.Song.Remove(song);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Artist");
        }

        private async void PopulatePlaylistsDropDownList(object selectedPlaylist = null)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            var user = await _context.User
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            var playlists = from p in _context.Playlist
                                where p.User == user
                                orderby p.Name
                                select p;
            var res = playlists.AsNoTracking();
            ViewBag.playlistId = new SelectList(res, "Id", "Nazwa", selectedPlaylist);
        }

//------------------------------------------------------------------------------------

// GET: Songs/AddToPlaylist/5
        public async Task<IActionResult> AddToPlaylist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            int? userId = HttpContext.Session.GetInt32("UserID");
            var user = await _context.User
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);
            var playlists = await _context.Playlist.Where(p => p.User == user).ToListAsync();

            var viewModel = new AddSongToPlaylistViewModel
            {
                SongId = song.SongId,
                Playlists = playlists.Select(p => new SelectListItem
                {
                    Value = p.PlaylistId.ToString(),
                    Text = p.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Songs/AddToPlaylist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToPlaylist(AddSongToPlaylistViewModel viewModel)
        {
                var playlist = await _context.Playlist
                    .Include(p => p.Songs)
                    .FirstOrDefaultAsync(p => p.PlaylistId == viewModel.SelectedPlaylistId);

                if (playlist == null)
                {
                    return NotFound();
                }

                var song = await _context.Song.FindAsync(viewModel.SongId);
                if (song == null)
                {
                    return NotFound();
                }

                playlist.Songs.Add(song);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Playlist");
        }

        public async Task<IActionResult> AddFromRecommended(int playlistId, int songId)
        {
                var playlist = await _context.Playlist
                    .Include(p => p.Songs)
                    .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);
                if (playlist == null)
                {
                    return NotFound();
                }
                var song = await _context.Song.FindAsync(songId);
                if (song == null)
                {
                    return NotFound();
                }
                playlist.Songs.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction("PlaylistIndex", "Song", new { playlistId });
        }

//-----------------------------------------------------------------------------------
        public async Task<IActionResult> AddToFavorites(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.User
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            if (!user.Favorites.Contains(song))
            {
                user.Favorites.Add(song);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Favorite", "User");
        }

//------------------------------------------------------------------------------------
    public async Task<IActionResult> RemoveFromPlaylist(int songId, int playlistId)
        {
            var playlist = await _context.Playlist
                .Include(p => p.Songs)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            if (playlist == null)
            {
                return NotFound();
            }

            var song = playlist.Songs.FirstOrDefault(s => s.SongId == songId);
            if (song != null)
            {
                playlist.Songs.Remove(song);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PlaylistIndex", "Song", new { playlistId });
        }

//-------------------------------------------------------------------------------------

        private bool SongExists(int id)
        {
          return (_context.Song?.Any(e => e.SongId == id)).GetValueOrDefault();
        }
    }
}
