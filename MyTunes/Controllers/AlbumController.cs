using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTunes.Data;
using MyTunes.Models;

namespace MyTunes.Controllers
{
    public class AlbumController : Controller
    {
        private readonly MyTunesContext _context;

        public AlbumController(MyTunesContext context)
        {
            _context = context;
        }

        [Route("/Album/logout")]
        public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

        // GET: Album
        public async Task<IActionResult> Index(int id)
        {
            var artist = await _context.Artist.FindAsync(id);
            ViewBag.ArtistId = id;
            ViewBag.Artist = artist;

            var albums = await _context.Album.Where(a => a.Artist == artist).ToListAsync();

            ViewData["Username"] = HttpContext.Session.GetString("Username");

            return View(albums);
            // return _context.Album.Where(a => a.Artist == artist) != null ? 
            //             View(await _context.Album.ToListAsync()) :
            //             Problem("Entity set 'MyTunesContext.Album'  is null.");
        } 

        // GET: Album/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Album/Create
        public IActionResult Create()
        {
            return View(); 
        }

        // POST: Album/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,Name,ReleaseDate,Genre")] Album album, int id)
        {
            var artist = _context.Artist.Where(a => a.ArtistId == id).First();
            album.Artist = artist;
            
            _context.Add(album);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Album", new {id = artist.ArtistId});  
        }

        // GET: Album/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = await _context.Album.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Album/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,Name,ReleaseDate,Genre")] Album album)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.AlbumId))
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
            return View(album);
        }

        // GET: Album/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Album/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Album == null)
            {
                return Problem("Entity set 'MyTunesContext.Album'  is null.");
            }
            var album = await _context.Album.FindAsync(id);
            if (album != null)
            {
                _context.Album.Remove(album);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Artist");
        }

        private bool AlbumExists(int id)
        {
          return (_context.Album?.Any(e => e.AlbumId == id)).GetValueOrDefault();
        }
    }
}
