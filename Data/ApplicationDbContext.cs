using Microsoft.EntityFrameworkCore;
using AssetTrack.Models;

namespace AssetTrack.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Asset> Assets { get; set; }

        public DbSet<Peminjaman> Peminjamans { get; set; }
    }
}