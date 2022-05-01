using System.Diagnostics;
using Desafio.Umbler.Api.Views;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Umbler.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }     

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
