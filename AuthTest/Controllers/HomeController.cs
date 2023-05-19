using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
