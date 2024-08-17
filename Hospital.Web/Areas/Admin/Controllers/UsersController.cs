using Hospital.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private IApplicationUserService _userService;

        public UsersController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index(int PageNumber = 1, int PageSize = 10)
        {
            var pagedResult = _userService.GetAll(PageNumber, PageSize);
            return View(pagedResult);
        }

        [HttpGet]
        public IActionResult AllDoctors(int PageNumber = 1, int PageSize = 10)
        {
            var pagedResult = _userService.GetAllDoctor(PageNumber, PageSize);
            return View(pagedResult);
        }
    }
}
