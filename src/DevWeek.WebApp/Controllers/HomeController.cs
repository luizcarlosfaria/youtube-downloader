using DevWeek.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DevWeek.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return this.View();
    }

    public IActionResult Sobre()
    {
        return this.View();
    }

    public IActionResult Contato()
    {
        return this.View();
    }

    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}
