using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BerberRandevu.Web.Models;

namespace BerberRandevu.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Ana sayfaya gelen herkesi login ekranına yönlendir
        return RedirectToAction("Giris", "Hesap");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
