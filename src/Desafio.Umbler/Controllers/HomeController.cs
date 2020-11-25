using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Desafio.Umbler.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Desafio.Umbler.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _db;

        public HomeController(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var domains = await _db.Domains.ToListAsync();

            return View(domains);
        }     

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
