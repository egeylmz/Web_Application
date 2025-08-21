namespace WebProject.Models
{
    public class Izin
    {
        public int Id { get; set; }
        public String? AdSoyad { get; set; }
        public String? IzinTipi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public String? Aciklama { get; set; }
        public String? Durum { get; set; }
        
    }

}
