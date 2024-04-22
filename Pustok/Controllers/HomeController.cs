using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pustok.Data;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class HomeController : Controller
{
    private AppDbContext _context;
    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        HomeViewModel homeVM = new HomeViewModel
        {
            Sliders = _context.Sliders.OrderBy(x => x.Order).ToList(),
        };
        return View(homeVM);
    }
}

