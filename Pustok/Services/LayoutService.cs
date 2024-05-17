using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Services
{
	public class LayoutService
	{
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(AppDbContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
        }
        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public Dictionary<String, String> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }
        //public BasketViewModel GetBasket()
        //{
        //    BasketViewModel vm = new BasketViewModel();
        //    if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.User.IsInRole("member"))
        //    {
        //        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //        var basketItems = _context.BasketItems
        //       .Include(x => x.Book)
        //       .ThenInclude(b => b.BookImages.Where(bi => bi.Status == true))
        //       .Where(x => x.AppUserId == userId)
        //       .ToList();

        //        vm.Items = basketItems.Select(x => new BasketItemViewModel
        //        {
        //            Id = x.Id,
        //            BookName = x.Book.Name,
        //            BookPrice = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice,
        //            BookImage = x.Book.BookImages.FirstOrDefault(x => x.Status == true)?.Name,
        //            Count = x.Count
        //        }).ToList();

        //        vm.TotalPrice = vm.Items.Sum(x => x.Count * x.BookPrice);
        //    }
        //    return vm;

        //}
    }
}

