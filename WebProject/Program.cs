using Microsoft.EntityFrameworkCore;
using WebProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Servisleri tanımla
builder.Services.AddDbContext<ApplicationDbContext>(options =>                                  //Kullanıcı ve izinlerin olduğu
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();



builder.Services.AddHttpClient("UsersApi", (sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["UsersApi:BaseUrl"] ?? throw new InvalidOperationException("UsersApi:BaseUrl missing");
    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    client.BaseAddress = new Uri(baseUrl);
    // API Key otomatik eklenmiyor - manuel kontrol için
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
})
.ConfigurePrimaryHttpMessageHandler(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    var h = new HttpClientHandler();
    if (env.IsDevelopment())
        h.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return h;
});

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.LoginPath = "/Home/Index";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(1);     //her kullanıcının oturumu 1dk sonra kapanacak.
    options.SlidingExpiration = false;                    //otomatik olarak süre uzamasın diye.
    options.Events.OnRedirectToLogin = context =>
    {
        context.HttpContext.Response.Redirect("/Home/Index?sessionExpired=true");
        return Task.CompletedTask;
    };
});


builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware sırası
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();       //static dosyaları çalıştırır.
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
