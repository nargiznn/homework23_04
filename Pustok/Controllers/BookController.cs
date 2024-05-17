using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;
using Pustok.ViewModels;
using System.Security.Claims;
using System.Text.Json;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CountService countService;
        private readonly CountManageService countManageService;
        private readonly UserManager<AppUser> _userManager;

        public BookController(AppDbContext context, CountService countService, CountManageService countManageService, UserManager<AppUser> userManager)
        {
            _context = context;
            this.countService = countService;
            this.countManageService = countManageService;
            _userManager = userManager;
        }
        public IActionResult GetBookById(int id)
        {
            Book book = _context.Books.Include(x => x.Genre).Include(x => x.BookImages.Where(x => x.Status == true)).FirstOrDefault(x => x.Id == id);
            return PartialView("_BookModalContentPartial", book);
        }

        //service lifecycle
        public IActionResult Add()
        {
            countService.Add();
            countService.Add();
            countService.Add();

            countManageService.Add();
            countManageService.Add();


            return Json(new { count = countService.Count });
        }


        public IActionResult Detail(int id)
        {
            var vm = getBookDetailVM(id);

            if (vm.Book == null) return RedirectToAction("notfound", "error");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Review(BookReview review)
        {
            AppUser? user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
                return RedirectToAction("login", "account", new { returnUrl = Url.Action("detail", "book", new { id = review.BookId }) });

            if (!_context.Books.Any(x => x.Id == review.BookId && !x.IsDeleted))
                return RedirectToAction("notfound", "error");

            if (_context.BookReviews.Any(x => x.Id == review.BookId && x.AppUserId == user.Id))
                return RedirectToAction("notfound", "error");


            if (!ModelState.IsValid)
            {
                var vm = getBookDetailVM(review.BookId);
                vm.Review = review;
                return View("detail", vm);
            }


            review.AppUserId = user.Id;
            review.CreatedAt = DateTime.Now;

            _context.BookReviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("detail", new { id = review.BookId });
        }


        private BookDetailViewModel getBookDetailVM(int bookId)
        {


            Book? book = _context.Books
              .Include(x => x.Genre)
              .Include(x => x.Author)
              .Include(x => x.BookImages)
              .Include(x => x.BookReviews.Take(2)).ThenInclude(r => r.AppUser)
              .Include(x => x.BookTags).ThenInclude(bt => bt.Tag)
              .FirstOrDefault(x => x.Id == bookId && !x.IsDeleted);

            BookDetailViewModel vm = new BookDetailViewModel
            {
                Book = book,
                RelatedBooks = _context.Books
                       .Include(x => x.Author)
                       .Include(x => x.BookImages
                               .Where(bi => bi.Status != null))
                       .Where(x => book != null && x.GenreId == book.GenreId)
                       .Take(5).ToList(),
                Review = new BookReview { BookId = bookId }
            };

            AppUser? user = _userManager.GetUserAsync(User).Result;

            if (_userManager.IsInRoleAsync(user, "member").Result && _context.BookReviews.Any(x => x.BookId == bookId && x.AppUserId == user.Id && x.Status != Models.Enum.ReviewStatus.Rejected))
            {
                vm.HasUserReview = true;
            }

            vm.TotalReviewsCount = _context.BookReviews.Count(x => x.BookId == bookId);
            vm.AvgRate = vm.TotalReviewsCount>0?(int)Math.Ceiling(_context.BookReviews.Where(x => x.BookId == bookId).Average(x => x.Rate)):0;

            return vm;
        }

        public IActionResult AddToBasket(int bookId)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == bookId && !x.IsDeleted);

            if (book == null) return RedirectToAction("notfound", "error");

            if (User.Identity.IsAuthenticated && User.IsInRole("member"))
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                BasketItem? basketItem = _context.BasketItems.FirstOrDefault(x => x.AppUserId == userId && x.BookId == bookId);

                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = userId,
                        BookId = bookId,
                        Count = 1
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else basketItem.Count++;

                _context.SaveChanges();
            }
            else
            {
                List<BasketCookieItemViewModel> basketItems = new List<BasketCookieItemViewModel>();

                var cookieItem = Request.Cookies["basket"];

                if (cookieItem != null)
                {
                    basketItems = JsonSerializer.Deserialize<List<BasketCookieItemViewModel>>(cookieItem);
                }

                BasketCookieItemViewModel item = basketItems.FirstOrDefault(x => x.BookId == bookId);

                if (item == null)
                {
                    item = new BasketCookieItemViewModel
                    {
                        BookId = bookId,
                        Count = 1
                    };
                    basketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }

                Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems));
            }

            return RedirectToAction("index", "home");
        }
    }
}