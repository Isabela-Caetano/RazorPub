using Microsoft.AspNetCore.Mvc;
using RazorPub.Models;
using System.Diagnostics;

namespace RazorPub.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Servicos() => View();
        public IActionResult Espaco() => View();
        public IActionResult Avaliacoes() => View();
        public IActionResult Contato() => View();

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Sobre()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
