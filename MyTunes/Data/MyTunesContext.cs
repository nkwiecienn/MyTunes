using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTunes.Models;

namespace MyTunes.Data
{
    public class MyTunesContext : DbContext
    {
        public MyTunesContext (DbContextOptions<MyTunesContext> options)
            : base(options)
        {
        }

        public DbSet<MyTunes.Models.Artist> Artist { get; set; } = default!;

        public DbSet<MyTunes.Models.Album> Album { get; set; } = default!;

        public DbSet<MyTunes.Models.Playlist> Playlist { get; set; } = default!;

        public DbSet<MyTunes.Models.Song> Song { get; set; } = default!;

        public DbSet<MyTunes.Models.User> User { get; set; } = default!;
    }
}
