using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebProject.Models;

namespace WebProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(bool sessionExpired = false)
    {
        if (sessionExpired)                     //oturum süresi dolduysa hata mesajı bastırılacak
        {
            TempData["Hata"] = "Oturum süreniz doldu. Lütfen tekrar giriş yapın.";
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]            //önbellekleme yapılmaması için, dinamik olacak
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            // Activity.Current?.Id .netcoreun otomatik oluşturduğu benzersiz id
    }
}
