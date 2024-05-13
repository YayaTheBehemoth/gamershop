using Microsoft.AspNetCore.Mvc;

namespace gamershop.Server.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
