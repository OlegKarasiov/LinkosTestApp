using Microsoft.AspNetCore.Mvc;

namespace LinkosTestApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
