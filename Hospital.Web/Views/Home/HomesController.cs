using Microsoft.AspNetCore.Mvc;

namespace Hospital.Web.Views.Home
{
    public class HomesController : Controller
    {
        [Area("HomeArea")]
        public IActionResult Index()
        {
            return RedirectToAction("Index");
        }
    }
}
