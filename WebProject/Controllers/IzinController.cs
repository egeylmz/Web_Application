using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;
using WebProject.Models;

[Authorize(Roles = "IzinKullanicisi")]                  
public class IzinController : Controller
{
    private readonly ApplicationDbContext _context;
    public IzinController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()                    //izin kullanıcısı paneli
    {
        var izinler = _context.Izinler.ToList();
        return View(izinler);
    }

    public IActionResult Yeni()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Yeni(Izin model)           //yeni izin talebi oluşturma 
    {
        // başlangıç ve bitiş tarihlerini kontrol et.
        if (model.BaslangicTarihi < DateTime.Today )
        {
            ModelState.AddModelError("", "Sadece ileri tarihli izin talepleri oluşturabilirsiniz.");
            return View(model);
        }
        if (model.BitisTarihi < DateTime.Today)
        {
            ModelState.AddModelError("", "Bitiş tarihi, geçmiş bir tarih olamaz.");
            return View(model);   
        }
        if (model.BitisTarihi < model.BaslangicTarihi)
        {
            ModelState.AddModelError("", "Bitiş tarihi, başlangıç tarihinden önce olamaz.");
            return View(model);
        }

        if (ModelState.IsValid)
        {
            model.AdSoyad = User.Identity?.Name;
            model.Durum = "Bekliyor";
            _context.Izinler.Add(model);
            _context.SaveChanges();
            return RedirectToAction("IzinListele");
        }
        return View(model);
    }

    public IActionResult IzinListele()              //sadece kendi izin talepleri görüntülenecek
    {
        var kullaniciAdi = User.Identity?.Name;

        var talepler = _context.Izinler
                               .Where(i => i.AdSoyad == kullaniciAdi)
                               .ToList();

        if (talepler == null || !talepler.Any())
        {
            return View("IzinTalepBulunamadi");
        }
        
        return View(talepler);
    }
}
