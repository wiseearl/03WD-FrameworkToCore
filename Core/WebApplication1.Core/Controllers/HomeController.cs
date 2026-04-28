using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core.Models;

namespace WebApplication1.Core.Controllers;

public class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("About")]
    public IActionResult About()
    {
        return View();
    }

    [HttpGet("Contact")]
    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
