using Microsoft.EntityFrameworkCore;
using WebProjectApi.Models;

namespace WebProjectApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
            
        }
        public DbSet<Kullanici> Kullanicilar { get; set; } = null!;
    }
}
