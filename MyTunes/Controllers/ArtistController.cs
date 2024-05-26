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
    public class ArtistController : Controller
    {
        private readonly MyTunesContext _context;

        public ArtistController(MyTunesContext context)
        {
            _context = context;
        }

        [Route("/Artist/logout")]
            public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Artist
        public async Task<IActionResult> Index()
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");

              return _context.Artist != null ? 
                          View(await _context.Artist.ToListAsync()) :
                          Problem("Entity set 'MyTunesContext.Artist'  is null.");
        }

        // GET: Artist/Details/5
        public IActionResult Details(int? id)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            return RedirectToAction("Index", "Album", new { id });
        }

        // GET: Artist/Create
        public IActionResult Create() 
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            return View();
        }

        // POST: Artist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtistId,Name,DebutDate")] Artist artist)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewBag.Artist = artist;
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artist/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        // POST: Artist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistId,Name,DebutDate")] Artist artist)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            if (id != artist.ArtistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.ArtistId))
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
            return View(artist);
        }

        // GET: Artist/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .FirstOrDefaultAsync(m => m.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            if (_context.Artist == null)
            {
                return Problem("Entity set 'MyTunesContext.Artist'  is null.");
            }
            var artist = await _context.Artist.FindAsync(id);
            if (artist != null)
            {
                _context.Artist.Remove(artist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            return (_context.Artist?.Any(e => e.ArtistId == id)).GetValueOrDefault();
        }
    }
}
