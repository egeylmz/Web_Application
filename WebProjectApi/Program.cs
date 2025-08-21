using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;        // Swagger
using WebProjectApi.Data;              // ApplicationDbContext
using WebProjectApi.Models;            // Kullanici

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// Swagger + X-API-KEY
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKeyAuth", new OpenApiSecurityScheme
    {
        Description = "API anahtarını girin.",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKeyAuth" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS (opsiyonel)
const string CorsPolicy = "allow-mvc";
var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);

app.MapControllers();

app.Run();
