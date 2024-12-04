using KuaforSalonu.Models;
using Microsoft.EntityFrameworkCore;

namespace KuaforSalonu.Data
{
    public class KuaforDbContext : DbContext
    {
        public KuaforDbContext(DbContextOptions<KuaforDbContext> options) : base(options) { }

        public DbSet<Salon> Salons { get; set; }
        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<Servis> Servisler { get; set; }
    }
}
