namespace WebProject.Models
{
    public class EkipmanTalep
    {
        public int Id { get; set; }
        public String? TalepKişi { get; set; }
        public String? EkipmanAdi { get; set; }
        public String? TalepSayisi { get; set; }
        public String? Aciklama { get; set; }
        public String? Durum { get; set; }
        public DateTime TalepTarihi{ get; set; }

    }
}