using Microsoft.AspNetCore.Mvc;

namespace Hospital.Web.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
