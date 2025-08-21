using Microsoft.EntityFrameworkCore;
using WebProject.Models;

namespace WebProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }              
        public DbSet<Izin> Izinler { get; set; }
        public DbSet<Ekipman> Ekipmanlar { get; set; }
        public DbSet<EkipmanTalep> EkipmanTalepleri { get; set; }
       
        // !!!!!!!!!!!!!!!!!!!!! Diğer DbSet'leri de buraya ekle !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }
}
