using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject.Data;
using WebProject.Models;

[Authorize(Roles = "Admin")]                        //admin paneline erişim izni
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _http;
    
    public AdminController(ApplicationDbContext context, IHttpClientFactory http)
    {
        _context = context;
        _http = http;
    }

    public IActionResult Panel()
    {
        return View();
    }
    public async Task<IActionResult> GuncelKullanicilar()
    {
        // API Key kontrolü - sadece admin kullanıcılar erişebilsin
        if (!User.IsInRole("Admin"))
        {
            TempData["Hata"] = "Bu sayfaya erişim yetkiniz yok.";
            return RedirectToAction("Panel");
        }

        // API Key durumunu kontrol et
        var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = configuration["UsersApi:ApiKey"];
        
        if (string.IsNullOrEmpty(apiKey))
        {
            TempData["Hata"] = "API Key konfigürasyonu bulunamadı. Lütfen sistem yöneticisi ile iletişime geçin.";
            return View(new List<Kullanici>());
        }

        var client = _http.CreateClient("UsersApi");
        
        // API Key'i manuel olarak ekle
        client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        
        try
        {
            using var res = await client.GetAsync("api/users", HttpCompletionOption.ResponseHeadersRead);
            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync();
                var hint = res.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => " (X-API-KEY eksik/yanlış olabilir.)",
                    HttpStatusCode.NotFound     => " (Route/port hatalı olabilir.)",
                    _ => ""
                };
                TempData["Hata"] = $"API hata: {(int)res.StatusCode} {res.ReasonPhrase}{hint} | Body: {body}";
                return View(new List<Kullanici>());
            }

            var list = await res.Content.ReadFromJsonAsync<List<Kullanici>>();
            return View(list ?? new List<Kullanici>());
        }
        catch (Exception ex)
        {
            TempData["Hata"] = "API çağrısı başarısız: " + ex.Message + " | Inner: " + ex.InnerException?.Message;
            return View(new List<Kullanici>());
        }
    }

    public IActionResult Kullanicilar()       //kullanıcıları listeleyecek
    {
        var kullanicilar = _context.Kullanicilar.ToList();
        return View(kullanicilar);
    }

    [HttpPost]
    // [ValidateAntiForgeryToken]  güvenlik için kullanılabilir
    public IActionResult YeniKullanici(Kullanici yeniKullanici)        //yeni kullanıcı oluşturma
    {
        if (ModelState.IsValid)
        {
            yeniKullanici.KayitTarihi = DateTime.Now;                  //kullanıcı kayıt tarihi otomatik olarak güncel tarih
            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();
            return Json(new { success = true });                       //ajax çağrısı ile başarı mesajı
        }
        return Json(new { success = false, message = "Eksik veya hatalı veri." });      //ajax çağrısı ile hata mesajı
    }

    public IActionResult IzinTalepleri()            //izin taleplerini görüntüleme
    {
        var izinler = _context.Izinler.ToList();
        return View(izinler);
    }

    public IActionResult EkipmanTalepleri()         //ekipman taleplerini görüntüleme
    {
        var talepler = _context.EkipmanTalepleri.ToList();
        return View(talepler);
    }

    public IActionResult EkipmanStok()              //ekipman stoklarını listeleyecek
    {
        var stoklar = _context.Ekipmanlar.ToList();
        return View(stoklar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult YeniStok([FromBody] Ekipman yeniStok)     //yeni ekipman stoğu ekleme
    {
        if (ModelState.IsValid)
        {
            _context.Ekipmanlar.Add(yeniStok);
            _context.SaveChanges();
            return Json(new { success = true});         //ajax çağrısı ile başarı mesajı
        }
        return Json(new { success = false});            //ajax çağrısı ile hata mesajı
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult IzinOnayla(int id)         //izin talebi onaylama
    {
        var izin = _context.Izinler.FirstOrDefault(i => i.Id == id);    
        if (izin != null && izin.Durum == "Bekliyor")           
        {
            izin.Durum = "Onaylandı";
            _context.SaveChanges();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "İzin talebi bulunamadı veya zaten işlenmiş." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TalepOnayla(int id)        //ekipman talebi onaylama
    {
        var talep = _context.EkipmanTalepleri.FirstOrDefault(i => i.Id == id);
        if (talep != null && talep.Durum == "Bekliyor")
        {
            talep.Durum = "Onaylandı";
            _context.SaveChanges();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Ekipman talebi bulunamadı veya zaten işlenmiş." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TalepReddet(int id)        //ekipman talebi reddetme
    {
        var talep = _context.EkipmanTalepleri.FirstOrDefault(i => i.Id == id);
        if (talep != null && talep.Durum == "Bekliyor")
        {
            talep.Durum = "Reddedildi";
            _context.SaveChanges();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Ekipman talebi bulunamadı veya zaten işlenmiş." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult IzinReddet(int id)         //izin talebi reddetme
    {
        var izin = _context.Izinler.FirstOrDefault(i => i.Id == id);
        if (izin != null && izin.Durum == "Bekliyor")
        {
            izin.Durum = "Reddedildi";
            _context.SaveChanges();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "İzin talebi bulunamadı veya zaten işlenmiş." });
    }


    [HttpGet]
    public IActionResult Duzenle(int id)            //kullanıcı bilgilerini düzenlemek için veriyi çekecek.
    {
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == id);
        if (kullanici == null)
        {
            return NotFound();
        }
        return View(kullanici);
    }

    [HttpPost]
    public IActionResult Duzenle(Kullanici guncelKullanici)         //kullanıcı bilgileri alıp günceller.
    {
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == guncelKullanici.Id);
        if (kullanici == null)
        {
            return NotFound();
        }
        kullanici.AdSoyad = guncelKullanici.AdSoyad;
        kullanici.Rol = guncelKullanici.Rol;
        kullanici.Aktif = guncelKullanici.Aktif;
        kullanici.Sifre = guncelKullanici.Sifre;

        _context.SaveChanges();
    return RedirectToAction("Kullanicilar");        //burada ajax kullanılmadı. sayfanın tamamı yenilenecek.
    }

    [HttpPost]                                 
    [ValidateAntiForgeryToken]                      //güvenlik için.
    public IActionResult SilKullanici(int id)       //kullanıcı silme işlemi
    {
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == id);
        if (kullanici == null)
        {                               
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }
        var girisYapanAd = User.Identity?.Name;
        if (kullanici.AdSoyad == girisYapanAd)
        {
            return Json(new { success = false, message = "Kendi hesabınızı silemezsiniz." });
        }
        _context.Kullanicilar.Remove(kullanici);
        _context.SaveChanges();
        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SilEkipman(int id)
    {
        var ekipman = _context.Ekipmanlar.FirstOrDefault(e => e.Id == id);
        if (ekipman == null)
        {
            return Json(new { success = false, message = "Ekipman bulunamadı." });
        }
        _context.Ekipmanlar.Remove(ekipman);
        _context.SaveChanges();
        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult EkipmanDuzenle(int id)               //ekipman düzenlemek için verileri alacak    
    {
        var ekipman = _context.Ekipmanlar.FirstOrDefault(e => e.Id == id);
        if (ekipman == null)
            return NotFound();

        return View(ekipman);
    }

    [HttpPost]

    public IActionResult EkipmanDuzenle(Ekipman guncel)       //bilgileri alıp güncelleyecek
    {
        var ekipman = _context.Ekipmanlar.FirstOrDefault(e => e.Id == guncel.Id);
        if (ekipman == null)
            return NotFound();

        ekipman.EkipmanAdi = guncel.EkipmanAdi;
        ekipman.StokAdedi = guncel.StokAdedi;
        _context.SaveChanges();
        return RedirectToAction("EkipmanStok");
    }
}
