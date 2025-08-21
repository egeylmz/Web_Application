using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProjectApi.Data;
using WebProjectApi.Filters;
using WebProjectApi.Models;

namespace WebProjectApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKey] // opsiyonel filtre
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext db) => _context = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kullanici>>> GetAll()
        {
            var list = await _context.Kullanicilar.AsNoTracking()
                         .OrderByDescending(x => x.Id).ToListAsync();
            return Ok(list);
        }
    }
}
