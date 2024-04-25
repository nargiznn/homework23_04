using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            FeaturedBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages.Where(bi => bi.Status != null)).Where(x => x.IsFeatured).Take(10).ToList(),
            NewBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages.Where(bi => bi.Status != null)).Where(x => x.IsNew).Take(10).ToList(),
            DiscountedBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages.Where(bi => bi.Status != null)).Where(x => x.DiscountPercent > 0).OrderByDescending(x => x.DiscountPercent).Take(10).ToList(),
        };
        return View(homeVM);
    }
}

