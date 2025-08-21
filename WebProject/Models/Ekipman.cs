namespace WebProject.Models;
using System.ComponentModel.DataAnnotations;

    public class Ekipman
    {
        public int Id { get; set; }   
        public String? EkipmanAdi { get; set; }
        [Required(ErrorMessage = "Ekipman adedi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Ekipman adedi bir sayı olmalıdır.")]
        public int StokAdedi { get; set; }
        
    }
