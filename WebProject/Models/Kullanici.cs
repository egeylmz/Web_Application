using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string? AdSoyad { get; set; }        // ? koymamızın sebebi bu alanlar null olabilir demek.
        public string? Rol { get; set; }            // Admin, IzinKullanici, EkipmanKullanici
        public string? Sifre { get; set; }
        public bool Aktif { get; set; }
        public DateTime KayitTarihi { get; set; }
    }

        
    
}
