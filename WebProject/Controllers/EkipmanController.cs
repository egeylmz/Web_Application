using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;
using WebProject.Models;

[Authorize(Roles = "EkipmanKullanicisi")]                       //ekipman kullanıcısının paneli
public class EkipmanController : Controller
{
    private readonly ApplicationDbContext _context;
    public EkipmanController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()                                //panel ekranı
    {
        var talepler = _context.EkipmanTalepleri.ToList();
        return View(talepler);
    }

    public IActionResult Yeni()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Yeni(EkipmanTalep model)       //Yeni Ekipman Talebi
    {
        if (ModelState.IsValid)
        {
            model.TalepKişi = User.Identity?.Name;
            model.TalepTarihi = DateTime.Now;
            model.Durum = "Bekliyor";
            _context.EkipmanTalepleri.Add(model);
            _context.SaveChanges();
            return RedirectToAction("EkipmanListele");
        }
        return View(model);
    }

    public IActionResult EkipmanListele()              //Ekipman talebi listeleme
    {
        var talepKisiAdi = User.Identity?.Name;
        var talepler = _context.EkipmanTalepleri.Where(i => i.TalepKişi == talepKisiAdi).ToList();

        if (talepler == null || !talepler.Any())       //talep yoksa
        {
            return View("EkipmanTalepBulunamadi");
        }
      
        return View(talepler);                    
    }

}
