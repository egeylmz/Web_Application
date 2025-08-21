using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using WebProject.Models;
using WebProject.Data;

namespace WebProject.Controllers                                                   
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;                 //readonly sadece bu class altında kullanılabilir yapıyor.

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()                         
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Kullanici model)          //ana ekranda kayıt olma kısmı
        {
            if (_context.Kullanicilar.Any(k => k.AdSoyad == model.AdSoyad))               //kullanıcı adı kontrolü
            {
                ModelState.AddModelError("AdSoyad", "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                model.Aktif = true;
                model.KayitTarihi = DateTime.Now;                   //kayıt tarihini otomatik olarak alacak

                _context.Kullanicilar.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");           //kayıt olduktan sonra tekrar ana giriş ekranına yönlendirecek.
            }

            return View(model);

        }

        //post ile formdan gelen verileri alacak.
        [HttpPost]
        public async Task<IActionResult> Login(string AdSoyad, string Sifre)        //asenkron çalışır. beklerken sistemi kilitlemez.
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.AdSoyad == AdSoyad && k.Sifre == Sifre && k.Aktif);
            //databaseden kullanıcı adı ve şifrelerini sorguluyor.

            if (kullanici != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kullanici.AdSoyad),
                    new Claim(ClaimTypes.Role, kullanici.Rol ?? "Kullanici")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");  //cookie authentication şeması
                var principal = new ClaimsPrincipal(identity);              //.net core un kullanıcıları tanıması için standard yapısu

                await HttpContext.SignInAsync("MyCookieAuth", principal, new AuthenticationProperties    //kullanıcı için cookie oluşturur, tarayıcıya gönderir
                {
                    IsPersistent = false,           //tarayıcı kapandığında cookei silinir
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1),   //1 dakika sonra oturum kapanacak
                    AllowRefresh = false            //oturum süresi her yeni işlemde YENİLENMEYECEK
                });
                // Oturum bitiş zamanını ayrı bir Cookie'ya ekleyecek
                HttpContext.Response.Cookies.Append("SessionEnd", DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());

                return kullanici.Rol switch                                 //Rollere göre ana panelleri değişiyor
                {
                    "Admin" => RedirectToAction("Panel", "Admin"),
                    "IzinKullanicisi" => RedirectToAction("Index", "Izin"),
                    "EkipmanKullanicisi" => RedirectToAction("Index", "Ekipman"),
                    _ => RedirectToAction("Index", "Home")                  // diğer kullanıcılar için ana sayfaya yönlendirecek
                };
            }
            TempData["Hata"] = "Giriş başarısız. Bilgilerinizi kontrol edin.";      
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Logout()       //kullanıcı oturumunu sonlandıracak.
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");       //tekrar ana giriş ekranına yönlendirecek
        }

        public IActionResult AccessDenied()             //yetkisi olmayan kullanıcıda bu gösterilecek. (normal kullanıcı admin sayfasına erişmeye çalışırsa.)
        {
            return Content("Bu sayfaya erişiminiz yok.");
        }

    }
}
