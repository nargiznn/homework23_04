using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pustok.Areas.Manage.Controllers
{
    [Authorize]
    [Area("manage")]
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
